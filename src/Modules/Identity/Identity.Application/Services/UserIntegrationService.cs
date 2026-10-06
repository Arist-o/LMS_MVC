using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Application.Common.Interfaces;
using Identity.Integration;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Services;

public class UserIntegrationService(IIdentityUnitOfWork unitOfWork) : IUserIntegrationService
{
    public async Task<string?> GetUserNameByIdAsync(Guid userId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId);
        return user != null ? $"{user.FirstName} {user.LastName} ({user.Role})" : null;
    }

    public async Task<IEnumerable<LookupDto>> GetInstructorsAsync()
    {
        var instructors = await unitOfWork.Users.Query()
            .Where(u => u.Role == "Instructor")
            .Select(u => new LookupDto(u.Id, $"{u.FirstName} {u.LastName} (Викладач)"))
            .ToListAsync();
        return instructors;
    }

    public async Task<IEnumerable<LookupDto>> GetStudentsAsync()
    {
        var students = await unitOfWork.Users.Query()
            .Where(u => u.Role == "Student")
            .Select(u => new LookupDto(u.Id, $"{u.FirstName} {u.LastName} (Студент)"))
            .ToListAsync();
        return students;
    }

    public async Task<Dictionary<Guid, string>> GetUserNamesByIdsAsync(IEnumerable<Guid> userIds)
    {
        var ids = userIds.Distinct().ToList();
        var users = await unitOfWork.Users.Query()
            .Where(u => ids.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}");
        return users;
    }
}
