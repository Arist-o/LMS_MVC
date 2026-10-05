using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Review.DTOs;
using MediatR;

namespace Courses.Application.Review.Queries;

public static class GetReviewById
{
    public sealed record Query(Guid Id) : IQuery<Result<ReviewResponseDto>>;

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ReviewResponseDto>>
    {
        public async Task<Result<ReviewResponseDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var review = await unitOfWork.Reviews.GetByIdAsync(request.Id, cancellationToken);
            if (review is null)
            {
                return Result<ReviewResponseDto>.Failure("Review not found.");
            }

            var response = new ReviewResponseDto(
                review.Id,
                review.CourseId,
                review.StudentId,
                review.Rating,
                review.Comment,
                review.IsApproved,
                review.CreatedAt
            );

            return Result<ReviewResponseDto>.Success(response);
        }
    }
}
