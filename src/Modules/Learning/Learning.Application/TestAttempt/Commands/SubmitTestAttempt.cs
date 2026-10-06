using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Integration;
using FluentValidation;
using Learning.Application.Common.Behaviors;
using Learning.Application.Common.Interfaces;
using Learning.Application.Common.Models;
using Learning.Application.TestAttempt.DTOs;
using MediatR;

namespace Learning.Application.TestAttempt.Commands;

public static class SubmitTestAttempt
{
    public sealed record Command(Guid StudentId, SubmitTestAttemptDto Dto) : ICommand<Result<TestAttemptResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.Dto.CourseId).NotEmpty();
            RuleFor(x => x.Dto.TestId).NotEmpty();
            RuleFor(x => x.Dto.Score).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler(ILearningUnitOfWork unitOfWork, ICourseIntegrationService courseIntegration) 
        : IRequestHandler<Command, Result<TestAttemptResponseDto>>
    {
        public async Task<Result<TestAttemptResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var testExists = await courseIntegration.TestExistsAsync(request.Dto.TestId, cancellationToken);
            if (!testExists)
            {
                return Result<TestAttemptResponseDto>.Failure("Test does not exist.");
            }

            var enrollment = await unitOfWork.Enrollments.FirstOrDefaultAsync(
                e => e.StudentId == request.StudentId && e.CourseId == request.Dto.CourseId, cancellationToken);

            if (enrollment == null)
            {
                return Result<TestAttemptResponseDto>.Failure("Student is not enrolled in this course.");
            }

            if (enrollment.Status != Domain.Entities.EnrollmentStatus.Active)
            {
                return Result<TestAttemptResponseDto>.Failure("Enrollment is not active.");
            }

            var attempt = Domain.Entities.TestAttempt.Create(enrollment.Id, request.Dto.TestId, request.Dto.Score);
            enrollment.AddTestAttempt(attempt);
            await unitOfWork.TestAttempts.AddAsync(attempt, cancellationToken);
            
            var dto = new TestAttemptResponseDto(attempt.Id, attempt.EnrollmentId, attempt.TestId, attempt.Score, attempt.AttemptDate);
            return Result<TestAttemptResponseDto>.Success(dto);
        }
    }
}
