using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Lesson.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lesson.Queries;

public static class GetLessons
{
    public sealed record Query(int Page = 1, int PageSize = 10) : IQuery<Result<PagedList<LessonResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PagedList<LessonResponseDto>>>
    {
        public async Task<Result<PagedList<LessonResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.Lessons.Query()
                .OrderBy(l => l.CourseId)
                .ThenBy(l => l.OrderIndex);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(l => new LessonResponseDto(
                    l.Id,
                    l.CourseId,
                    l.Title,
                    l.VideoUrl,
                    l.OrderIndex,
                    l.CreatedAt
                ))
                .ToListAsync(cancellationToken);

            var pagedList = new PagedList<LessonResponseDto>(items, totalCount, request.Page, request.PageSize);
            return Result<PagedList<LessonResponseDto>>.Success(pagedList);
        }
    }
}
