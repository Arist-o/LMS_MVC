using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using Identity.Application.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Users.Queries;

public static class GetUsers
{
    public sealed record Query(
        int Page = 1,
        int PageSize = 10,
        string? Search = null) : IRequest<Result<PagedList<UserDto>>>;

    public sealed class Handler(IIdentityUnitOfWork unitOfWork, AutoMapper.IMapper mapper) : IRequestHandler<Query, Result<PagedList<UserDto>>>
    {
        public async Task<Result<PagedList<UserDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.Users.Query();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.ToLower();
                query = query.Where(u => u.Email.ToLower().Contains(searchTerm) ||
                                         u.FirstName.ToLower().Contains(searchTerm) ||
                                         u.LastName.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var dbItems = await query
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items = dbItems.Select(mapper.Map<UserDto>).ToList();

            var pagedList = new PagedList<UserDto>(items, totalCount, request.Page, request.PageSize);
            return Result<PagedList<UserDto>>.Success(pagedList);
        }
    }
}
