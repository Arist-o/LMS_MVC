using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Course.DTOs;
using MediatR;

namespace Courses.Application.Course.Queries;

public static class GetCourseById
{
    public sealed record Query(Guid Id) : IQuery<Result<CourseResponseDto>>;

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<CourseResponseDto>>
    {
        public async Task<Result<CourseResponseDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var course = await unitOfWork.Courses.GetByIdAsync(request.Id, cancellationToken);
            if (course == null)
            {
                return Result<CourseResponseDto>.Failure($"Course with ID {request.Id} not found.");
            }

            var dto = new CourseResponseDto(
                course.Id,
                course.InstructorId,
                course.Title,
                course.Description,
                course.IsPublished,
                course.CreatedAt);

            return Result<CourseResponseDto>.Success(dto);
        }
    }
}
