using AutoMapper;
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
    public sealed record Query(GetLessonsDto Dto) : IQuery<Result<PagedList<LessonResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<PagedList<LessonResponseDto>>>
    {
        public async Task<Result<PagedList<LessonResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.Lessons.Query()
                .Include(l => l.Course)
                .OrderBy(l => l.CourseId)
                .ThenBy(l => l.OrderIndex);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.Dto.Page - 1) * request.Dto.PageSize)
                .Take(request.Dto.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = mapper.Map<System.Collections.Generic.List<LessonResponseDto>>(items);
            var pagedList = new PagedList<LessonResponseDto>(dtos, totalCount, request.Dto.Page, request.Dto.PageSize);
            return Result<PagedList<LessonResponseDto>>.Success(pagedList);
        }
    }
}
