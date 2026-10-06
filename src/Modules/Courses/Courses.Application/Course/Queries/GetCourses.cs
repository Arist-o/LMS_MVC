using AutoMapper;
using Courses.Application.Common.Interfaces;
using Courses.Application.Common.Models;
using Courses.Application.Course.DTOs;
using Identity.Integration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Course.Queries;

public static class GetCourses
{
    public sealed record Query(
        int Page = 1, 
        int PageSize = 10, 
        string? Search = null, 
        string? SortBy = null) : IRequest<Result<PagedList<CourseResponseDto>>>;

    public sealed class Handler(IUnitOfWork unitOfWork, IUserIntegrationService userIntegrationService, IMapper mapper) : IRequestHandler<Query, Result<PagedList<CourseResponseDto>>>
    {
        public async Task<Result<PagedList<CourseResponseDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.Courses.Query();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.ToLower();
                query = query.Where(c => c.Title.ToLower().Contains(searchTerm) || 
                                         c.Description.ToLower().Contains(searchTerm));
            }

            query = request.SortBy?.ToLower() switch
            {
                "title_desc" => query.OrderByDescending(c => c.Title),
                "title_asc" => query.OrderBy(c => c.Title),
                _ => query.OrderBy(c => c.Id)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var dbItems = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var instructorIds = dbItems.Select(i => i.InstructorId).Distinct().ToList();
            var instructorNames = await userIntegrationService.GetUserNamesByIdsAsync(instructorIds);

            var items = dbItems.Select(c => new CourseResponseDto(
                c.Id, 
                c.InstructorId, 
                instructorNames.GetValueOrDefault(c.InstructorId, "Unknown"),
                c.Title, 
                c.Description, 
                c.IsPublished, 
                c.CreatedAt)).ToList();

            var pagedList = new PagedList<CourseResponseDto>(items, totalCount, request.Page, request.PageSize);

            return Result<PagedList<CourseResponseDto>>.Success(pagedList);
        }
    }
}
