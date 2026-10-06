using AutoMapper;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using MediatR;

namespace Courses.Application.Course.Commands;

public static class DeleteCourse
{
    public sealed record Command(Guid Id) : ICommand<Result<bool>>;

    public sealed class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var course = await unitOfWork.Courses.GetByIdAsync(request.Id, cancellationToken);
            if (course == null)
            {
                return Result<bool>.Failure($"Course with ID {request.Id} not found.");
            }

            unitOfWork.Courses.Remove(course);
            return Result<bool>.Success(true);
        }
    }
}
