using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Lesson.DTOs;
using FluentValidation;
using MediatR;

namespace Courses.Application.Lesson.Commands;

public static class UpdateLesson
{
    public sealed record Command(UpdateLessonDto Dto) : ICommand<Result<LessonResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Id).NotEmpty().WithMessage("Lesson ID cannot be empty.");
            RuleFor(x => x.Dto.CourseId).NotEmpty().WithMessage("Course ID cannot be empty.");
            RuleFor(x => x.Dto.Title).NotEmpty().WithMessage("Title cannot be empty.");
            RuleFor(x => x.Dto.VideoUrl).NotEmpty().WithMessage("Video URL cannot be empty.");
            RuleFor(x => x.Dto.OrderIndex).GreaterThanOrEqualTo(0).WithMessage("Order index must be 0 or greater.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<LessonResponseDto>>
    {
        public async Task<Result<LessonResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var lesson = await unitOfWork.Lessons.GetByIdAsync(request.Dto.Id, cancellationToken);
            if (lesson == null)
            {
                return Result<LessonResponseDto>.Failure("Lesson not found.");
            }

            if (lesson.CourseId != request.Dto.CourseId)
            {
                var courseExists = await unitOfWork.Courses.AnyAsync(c => c.Id == request.Dto.CourseId, cancellationToken);
                if (!courseExists)
                {
                    return Result<LessonResponseDto>.Failure("Course not found.");
                }
                lesson.MoveToCourse(request.Dto.CourseId);
            }

            lesson.UpdateTitle(request.Dto.Title);
            lesson.UpdateVideoUrl(request.Dto.VideoUrl);
            lesson.UpdateOrderIndex(request.Dto.OrderIndex);
            lesson.UpdatedAtTime();

            unitOfWork.Lessons.Update(lesson);
            return Result<LessonResponseDto>.Success(new LessonResponseDto(
                lesson.Id,
                lesson.CourseId,
                lesson.Title,
                lesson.VideoUrl,
                lesson.OrderIndex,
                lesson.CreatedAt
            ));
        }
    }
}
