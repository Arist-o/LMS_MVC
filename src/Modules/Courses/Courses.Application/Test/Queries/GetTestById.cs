using AutoMapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Behaviors;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Test.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Test.Queries;

public static class GetTestById
{
    public sealed record Query(Guid Id) : IQuery<Result<TestResponseDto>>;

    public sealed class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<TestResponseDto>>
    {
        public async Task<Result<TestResponseDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var test = await unitOfWork.Tests.Query(asNoTracking: true)
                .Include(t => t.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (test == null)
            {
                return Result<TestResponseDto>.Failure($"Test with ID {request.Id} not found.");
            }

            var questionDtos = mapper.Map<List<QuestionResponseDto>>(test.Questions);

            return Result<TestResponseDto>.Success(mapper.Map<TestResponseDto>(test));
        }
    }
}
