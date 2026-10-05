using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lesson.Queries;

public static class GetLessonsLookup
{
    public sealed record Query() : IRequest<Result<List<LookupDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<List<LookupDto>>>
    {
        public async Task<Result<List<LookupDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var lessons = await unitOfWork.Lessons.Query()
                .Select(l => new LookupDto(l.Id, l.Title))
                .ToListAsync(cancellationToken);

            return Result<List<LookupDto>>.Success(lessons);
        }
    }
}
