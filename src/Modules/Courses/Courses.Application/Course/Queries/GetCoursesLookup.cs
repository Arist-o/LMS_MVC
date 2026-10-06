using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Course.Queries;

public static class GetCoursesLookup
{
    public sealed record Query() : IRequest<Result<List<LookupDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<List<LookupDto>>>
    {
        public async Task<Result<List<LookupDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var courses = await unitOfWork.Courses.Query()
                .Select(c => new LookupDto(c.Id, c.Title))
                .ToListAsync(cancellationToken);

            return Result<List<LookupDto>>.Success(courses);
        }
    }
}
