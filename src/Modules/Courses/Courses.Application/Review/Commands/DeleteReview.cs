using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace Courses.Application.Review.Commands;

public static class DeleteReview
{
    public sealed record Command(Guid Id) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Review ID cannot be empty.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var review = await unitOfWork.Reviews.GetByIdAsync(request.Id, cancellationToken);
            if (review is null)
            {
                return Result<bool>.Failure("Review not found.");
            }

            unitOfWork.Reviews.Remove(review);
            return Result<bool>.Success(true);
        }
    }
}
