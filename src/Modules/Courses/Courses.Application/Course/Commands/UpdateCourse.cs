using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Course.DTOs;
using FluentValidation;
using MediatR;

namespace Courses.Application.Course.Commands;

public static class UpdateCourse
{
    public sealed record Command(UpdateCourseDto Dto) : ICommand<Result<CourseResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Id).NotEmpty().WithMessage("Course ID cannot be empty.");
            RuleFor(x => x.Dto.Title).NotEmpty().WithMessage("Title cannot be empty.");
            RuleFor(x => x.Dto.Description).NotEmpty().WithMessage("Description cannot be empty.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<CourseResponseDto>>
    {
        public async Task<Result<CourseResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var course = await unitOfWork.Courses.GetByIdAsync(request.Dto.Id, cancellationToken);
            if (course == null)
            {
                return Result<CourseResponseDto>.Failure($"Course with ID {request.Dto.Id} not found.");
            }

            course.UpdateTitle(request.Dto.Title);
            course.UpdateDescription(request.Dto.Description);
            course.UpdateIsPublished(request.Dto.IsPublished);
            course.UpdateAtTime();

            unitOfWork.Courses.Update(course);
            return Result<CourseResponseDto>.Success(new CourseResponseDto(
                course.Id,
                course.InstructorId,
                course.Title,
                course.Description,
                course.IsPublished,
                course.CreatedAt));
        }
    }
}
