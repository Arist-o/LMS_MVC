using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Models;
using MediatR;

namespace Courses.Application.Course.Queries;

public static class GetStudentsLookup
{
    public sealed record Query() : IRequest<Result<List<LookupDto>>>;

    public sealed class Handler : IRequestHandler<Query, Result<List<LookupDto>>>
    {
        public Task<Result<List<LookupDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            // STUB: To be replaced with integration call to Identity module later
            var stub = new List<LookupDto>
            {
                new(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Тарас Шевченко (Студент)"),
                new(Guid.Parse("44444444-4444-4444-4444-444444444444"), "Михайло Грушевський (Студент)")
            };

            return Task.FromResult(Result<List<LookupDto>>.Success(stub));
        }
    }
}
