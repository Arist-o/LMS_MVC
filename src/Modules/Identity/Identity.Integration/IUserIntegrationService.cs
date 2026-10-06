using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Identity.Integration;

public record LookupDto(Guid Id, string Name);

public interface IUserIntegrationService
{
    Task<string?> GetUserNameByIdAsync(Guid userId);
    Task<IEnumerable<LookupDto>> GetInstructorsAsync();
    Task<IEnumerable<LookupDto>> GetStudentsAsync();
    Task<Dictionary<Guid, string>> GetUserNamesByIdsAsync(IEnumerable<Guid> userIds);
}
