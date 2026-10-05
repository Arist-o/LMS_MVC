using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Test.DTOs;
using FluentValidation;
using MediatR;

namespace Courses.Application.Test.Commands;

public static class UpdateTest
{
    public sealed record Command(UpdateTestDto Dto) : ICommand<Result<TestResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Id).NotEmpty().WithMessage("Id cannot be empty.");
            RuleFor(x => x.Dto.LessonId).NotEmpty().WithMessage("Lesson ID cannot be empty.");
            RuleFor(x => x.Dto.Title).NotEmpty().WithMessage("Title cannot be empty.");
            RuleFor(x => x.Dto.MaxScore).GreaterThanOrEqualTo(0).WithMessage("Max Score cannot be negative.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<TestResponseDto>>
    {
        public async Task<Result<TestResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var test = await unitOfWork.Tests.GetByIdAsync(request.Dto.Id, cancellationToken);
            if (test == null)
            {
                return Result<TestResponseDto>.Failure($"Test with ID {request.Dto.Id} not found.");
            }

            test.UpdateLessonId(request.Dto.LessonId);
            test.UpdateTitle(request.Dto.Title);
            test.UpdateMaxScore(request.Dto.MaxScore);
            test.UpdatedAtTime();

            unitOfWork.Tests.Update(test);
            return Result<TestResponseDto>.Success(new TestResponseDto(
                test.Id,
                test.LessonId,
                test.Title,
                test.MaxScore,
                test.CreatedAt,
                test.UpdatedAt
            ));
        }
    }
}
