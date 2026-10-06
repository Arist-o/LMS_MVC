using Learning.Application.Common.Interfaces;
using Learning.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Learning.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddLearningPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LearningDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionSqlServer")));

        services.AddScoped<ILearningUnitOfWork, LearningUnitOfWork>();

        return services;
    }
}
