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

public static class GetTests
{
    public sealed record Query(
        int Page = 1,
        int PageSize = 10,
        string? Search = null,
        string? SortBy = null) : IQuery<Result<PagedList<TestResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<PagedList<TestResponseDto>>>
    {
        public async Task<Result<PagedList<TestResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.Tests.Query();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(searchTerm));
            }

            query = request.SortBy?.ToLower() switch
            {
                "title_desc" => query.OrderByDescending(t => t.Title),
                "title_asc" => query.OrderBy(t => t.Title),
                "score_desc" => query.OrderByDescending(t => t.MaxScore),
                "score_asc" => query.OrderBy(t => t.MaxScore),
                _ => query.OrderBy(t => t.Id)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TestResponseDto(t.Id, t.LessonId, t.Title, t.MaxScore, t.CreatedAt, t.UpdatedAt))
                .ToListAsync(cancellationToken);

            var pagedList = new PagedList<TestResponseDto>(items, totalCount, request.Page, request.PageSize);

            return Result<PagedList<TestResponseDto>>.Success(pagedList);
        }
    }
}
