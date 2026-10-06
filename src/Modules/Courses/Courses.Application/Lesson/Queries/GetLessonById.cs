using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Lesson.DTOs;
using MediatR;

namespace Courses.Application.Lesson.Queries;

public static class GetLessonById
{
    public sealed record Query(Guid Id) : IQuery<Result<LessonResponseDto>>;

    public sealed class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<LessonResponseDto>>
    {
        public async Task<Result<LessonResponseDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var lesson = await unitOfWork.Lessons.GetByIdAsync(request.Id, cancellationToken);
            if (lesson == null)
            {
                return Result<LessonResponseDto>.Failure("Lesson not found.");
            }

            return Result<LessonResponseDto>.Success(mapper.Map<LessonResponseDto>(lesson));
        }
    }
}
