using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Test.DTOs;
using MediatR;

namespace Courses.Application.Test.Queries;

public static class GetTestById
{
    public sealed record Query(Guid Id) : IQuery<Result<TestResponseDto>>;

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<TestResponseDto>>
    {
        public async Task<Result<TestResponseDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var test = await unitOfWork.Tests.GetByIdAsync(request.Id, cancellationToken);
            if (test == null)
            {
                return Result<TestResponseDto>.Failure($"Test with ID {request.Id} not found.");
            }

            return Result<TestResponseDto>.Success(new TestResponseDto(
                test.Id,
                test.LessonId,
                test.Title,
                test.MaxScore,
                test.CreatedAt,
                test.UpdatedAt
            ));
        }
    }
}
