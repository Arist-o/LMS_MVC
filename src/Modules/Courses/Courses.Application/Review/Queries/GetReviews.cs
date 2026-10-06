using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Review.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Review.Queries;

public static class GetReviews
{
    public sealed record Query(int Page = 1, int PageSize = 10) : IQuery<Result<PagedList<ReviewResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<PagedList<ReviewResponseDto>>>
    {
        public async Task<Result<PagedList<ReviewResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.Reviews.Query();
            var totalCount = await query.CountAsync(cancellationToken);

            var reviews = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ReviewResponseDto(
                    x.Id,
                    x.CourseId,
                    x.StudentId,
                    x.Rating,
                    x.Comment,
                    x.IsApproved,
                    x.CreatedAt
                ))
                .ToListAsync(cancellationToken);

            var pagedList = new PagedList<ReviewResponseDto>(reviews, totalCount, request.Page, request.PageSize);
            return Result<PagedList<ReviewResponseDto>>.Success(pagedList);
        }
    }
}
