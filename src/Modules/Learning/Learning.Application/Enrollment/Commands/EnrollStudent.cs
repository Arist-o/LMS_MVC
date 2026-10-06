using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Integration;
using FluentValidation;
using Learning.Application.Common.Behaviors;
using Learning.Application.Common.Interfaces;
using Learning.Application.Common.Models;
using Learning.Application.Enrollment.DTOs;
using MediatR;

namespace Learning.Application.Enrollment.Commands;

public static class EnrollStudent
{
    public sealed record Command(Guid StudentId, EnrollStudentDto Dto) : ICommand<Result<EnrollmentResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.StudentId).NotEmpty();
            RuleFor(x => x.Dto.CourseId).NotEmpty();
        }
    }

    public sealed class Handler(ILearningUnitOfWork unitOfWork, ICourseIntegrationService courseIntegration) 
        : IRequestHandler<Command, Result<EnrollmentResponseDto>>
    {
        public async Task<Result<EnrollmentResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var courseExists = await courseIntegration.CourseExistsAsync(request.Dto.CourseId, cancellationToken);
            if (!courseExists)
            {
                return Result<EnrollmentResponseDto>.Failure("Course does not exist.");
            }

            var existing = await unitOfWork.Enrollments.FirstOrDefaultAsync(
                e => e.StudentId == request.StudentId && e.CourseId == request.Dto.CourseId, cancellationToken);
            if (existing != null)
            {
                return Result<EnrollmentResponseDto>.Failure("Already enrolled in this course.");
            }

            var enrollment = Domain.Entities.Enrollment.Create(request.StudentId, request.Dto.CourseId);
            await unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);
            // SaveChanges is handled by TransactionBehavior

            var dto = new EnrollmentResponseDto(enrollment.Id, enrollment.CourseId, enrollment.EnrollmentDate, enrollment.Status.ToString());
            return Result<EnrollmentResponseDto>.Success(dto);
        }
    }
}
