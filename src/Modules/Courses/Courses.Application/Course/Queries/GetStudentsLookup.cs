using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Models;
using Identity.Integration;
using MediatR;

namespace Courses.Application.Course.Queries;

public static class GetStudentsLookup
{
    public sealed record Query() : IRequest<Result<List<Courses.Application.Common.Models.LookupDto>>>;

    public sealed class Handler(IUserIntegrationService userIntegrationService) : IRequestHandler<Query, Result<List<Courses.Application.Common.Models.LookupDto>>>
    {
        public async Task<Result<List<Courses.Application.Common.Models.LookupDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var students = await userIntegrationService.GetStudentsAsync();
            var result = students.Select(i => new Courses.Application.Common.Models.LookupDto(i.Id, i.Name)).ToList();
            return Result<List<Courses.Application.Common.Models.LookupDto>>.Success(result);
        }
    }
}
