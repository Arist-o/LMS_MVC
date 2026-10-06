using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
            RuleFor(x => x.Dto.TestId).NotEmpty();
        }
    }

    public sealed class Handler(ILearningUnitOfWork unitOfWork, ICourseIntegrationService courseIntegration, IMapper mapper) 
        : IRequestHandler<Command, Result<TestAttemptResponseDto>>
    {
        public async Task<Result<TestAttemptResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var evaluationResult = await courseIntegration.EvaluateTestAsync(request.Dto.TestId, request.Dto.Answers, cancellationToken);
            if (evaluationResult == null)
            {
                return Result<TestAttemptResponseDto>.Failure("Failed to evaluate test.");
            }

            var courseId = evaluationResult.CourseId;
            var score = evaluationResult.Score;

            var enrollment = await unitOfWork.Enrollments.FirstOrDefaultAsync(
                e => e.StudentId == request.StudentId && e.CourseId == courseId, cancellationToken);

            if (enrollment == null)
            {
                return Result<TestAttemptResponseDto>.Failure("Student is not enrolled in this course.");
            }

            if (enrollment.Status != Domain.Entities.EnrollmentStatus.Active)
            {
                return Result<TestAttemptResponseDto>.Failure("Enrollment is not active.");
            }

            var attempt = Domain.Entities.TestAttempt.Create(enrollment.Id, request.Dto.TestId, score);
            enrollment.AddTestAttempt(attempt);
            await unitOfWork.TestAttempts.AddAsync(attempt, cancellationToken);
            
            var dto = mapper.Map<TestAttemptResponseDto>(attempt);
            return Result<TestAttemptResponseDto>.Success(dto);
        }
    }
}
