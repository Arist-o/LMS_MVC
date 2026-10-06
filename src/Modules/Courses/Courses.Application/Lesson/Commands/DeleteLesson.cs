using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using MediatR;

namespace Courses.Application.Lesson.Commands;

public static class DeleteLesson
{
    public sealed record Command(Guid Id) : ICommand<Result<bool>>;

    public sealed class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var lesson = await unitOfWork.Lessons.GetByIdAsync(request.Id, cancellationToken);
            if (lesson == null)
            {
                return Result<bool>.Failure("Lesson not found.");
            }

            unitOfWork.Lessons.Remove(lesson);
            return Result<bool>.Success(true);
        }
    }
}
