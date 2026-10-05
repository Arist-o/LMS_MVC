using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Review.DTOs;
using FluentValidation;
using MediatR;

namespace Courses.Application.Review.Commands;

public static class UpdateReview
{
    public sealed record Command(UpdateReviewDto Dto) : ICommand<Result<ReviewResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Id).NotEmpty().WithMessage("Review ID cannot be empty.");
            RuleFor(x => x.Dto.Rating).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
            RuleFor(x => x.Dto.Comment).MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");
            RuleFor(x => x.Dto.Comment).NotEmpty().WithMessage("Comment cannot be empty.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<ReviewResponseDto>>
    {
        public async Task<Result<ReviewResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var review = await unitOfWork.Reviews.GetByIdAsync(request.Dto.Id, cancellationToken);
            if (review is null)
            {
                return Result<ReviewResponseDto>.Failure("Review not found.");
            }

            review.UpdateRating(request.Dto.Rating);
            review.UpdateComment(request.Dto.Comment);
            review.UpdateIsApproved(request.Dto.IsApproved);
            review.UpdatedAtTime();

            unitOfWork.Reviews.Update(review);
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
