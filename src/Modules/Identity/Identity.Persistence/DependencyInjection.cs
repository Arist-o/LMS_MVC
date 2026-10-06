using Identity.Application.Auth.Services;
using Identity.Application.Common.Interfaces;
using Identity.Persistence.Auth;
using Identity.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionSqlServer")));

        services.AddScoped<IIdentityUnitOfWork, IdentityUnitOfWork>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        return services;
    }
}
