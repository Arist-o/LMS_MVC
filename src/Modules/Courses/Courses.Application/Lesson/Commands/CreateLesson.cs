using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Lesson.DTOs;
using FluentValidation;
using MediatR;

namespace Courses.Application.Lesson.Commands;

public static class CreateLesson
{
    public sealed record Command(CreateLessonDto Dto) : ICommand<Result<LessonResponseDto>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.CourseId).NotEmpty().WithMessage("Course ID cannot be empty.");
            RuleFor(x => x.Dto.Title).NotEmpty().WithMessage("Title cannot be empty.");
            RuleFor(x => x.Dto.VideoUrl).NotEmpty().WithMessage("Video URL cannot be empty.");
            RuleFor(x => x.Dto.OrderIndex).GreaterThanOrEqualTo(0).WithMessage("Order index must be 0 or greater.");
        }
    }

    public sealed class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result<LessonResponseDto>>
    {
        public async Task<Result<LessonResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var courseExists = await unitOfWork.Courses.AnyAsync(c => c.Id == request.Dto.CourseId, cancellationToken);
            if (!courseExists)
            {
                return Result<LessonResponseDto>.Failure("Course not found.");
            }

            var lesson = new Domain.Entities.Lesson(
                request.Dto.CourseId,
                request.Dto.Title,
                request.Dto.VideoUrl,
                request.Dto.OrderIndex
            );

            await unitOfWork.Lessons.AddAsync(lesson, cancellationToken);
            return Result<LessonResponseDto>.Success(mapper.Map<LessonResponseDto>(lesson));
        }
    }
}
