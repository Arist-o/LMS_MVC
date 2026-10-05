using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Models;
using MediatR;

namespace Courses.Application.Course.Queries;

public static class GetInstructorsLookup
{
    public sealed record Query() : IRequest<Result<List<LookupDto>>>;

    public sealed class Handler : IRequestHandler<Query, Result<List<LookupDto>>>
    {
        public Task<Result<List<LookupDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            // STUB: To be replaced with integration call to Identity module later
            var stub = new List<LookupDto>
            {
                new(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Іван Франко (Викладач)"),
                new(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Леся Українка (Викладач)")
            };

            return Task.FromResult(Result<List<LookupDto>>.Success(stub));
        }
    }
}
