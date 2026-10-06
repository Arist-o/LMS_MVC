using System.Threading.Tasks;
using Identity.Domain.Entities;

namespace Identity.Application.Auth.Services;

public interface IJwtProvider
{
    string Generate(User user);
}
