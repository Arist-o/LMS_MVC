using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using FluentValidation;
using MediatR;

namespace Courses.Application.Test.Commands;

public static class DeleteTest
{
    public sealed record Command(Guid Id) : ICommand<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id cannot be empty.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var test = await unitOfWork.Tests.GetByIdAsync(request.Id, cancellationToken);
            if (test == null)
            {
                return Result<bool>.Failure($"Test with ID {request.Id} not found.");
            }

            unitOfWork.Tests.Remove(test);
            return Result<bool>.Success(true);
        }
    }
}
