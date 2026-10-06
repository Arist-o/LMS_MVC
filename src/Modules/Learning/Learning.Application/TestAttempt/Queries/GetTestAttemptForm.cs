using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Integration;
using Learning.Application.Common.Models;
using MediatR;

namespace Learning.Application.TestAttempt.Queries;

public static class GetTestAttemptForm
{
    public sealed record Query(Guid TestId) : IRequest<Result<TestAttemptFormDto>>;

    public sealed class Handler(ICourseIntegrationService courseIntegrationService) : IRequestHandler<Query, Result<TestAttemptFormDto>>
    {
        public async Task<Result<TestAttemptFormDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await courseIntegrationService.GetTestAttemptDataAsync(request.TestId, cancellationToken);
                return Result<TestAttemptFormDto>.Success(data);
            }
            catch (Exception ex)
            {
                return Result<TestAttemptFormDto>.Failure(ex.Message);
            }
        }
    }
}
