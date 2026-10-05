using System;
using System.Collections.Generic;
using System.Text;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Review.DTOs;
using DomainReview = Courses.Domain.Entities.Review;
using FluentValidation;
using MediatR;

namespace Courses.Application.Review.Commands
{
    public static class CreateReview
    {
        public sealed record Command(CreateReviewDto Dto) : ICommand<Result<ReviewResponseDto>>;

        public sealed class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Dto.CourseId).NotEmpty().WithMessage("Course ID cannot be empty.");
                RuleFor(x => x.Dto.StudentId).NotEmpty().WithMessage("Student ID cannot be empty.");
                RuleFor(x => x.Dto.Rating).InclusiveBetween(1 , 5).WithMessage("Rating must be between 1 and 5.");
                RuleFor(x => x.Dto.Comment).MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");
                RuleFor(x => x.Dto.Comment).NotEmpty().WithMessage("Comment cannot be empty.");
            }
        }

        public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<ReviewResponseDto>>
        {
            public async Task<Result<ReviewResponseDto>> Handle(Command request,CancellationToken cancellationToken)
            {
                var review = new DomainReview(
                    request.Dto.CourseId,
                    request.Dto.StudentId,
                    request.Dto.Rating,
                    request.Dto.Comment,
                    request.Dto.IsApproved
                );
                unitOfWork.Reviews.AddAsync(review);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<ReviewResponseDto>.Success(new ReviewResponseDto(review.CourseId, 
                    review.StudentId, 
                    review.Rating,
                    review.Comment, 
                    review.IsApproved, 
                    review.CreatedAt));
            }
        }
    }
}
