using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Courses.Persistence.Context;
using Courses.Application.Common.Interfaces;
using Courses.Persistence.Repositories;

namespace Courses.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoursesPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CoursesDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionSqlServer"),
                    sql =>
                    {
                        sql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                            );

                        sql.MigrationsAssembly(typeof(CoursesDbContext).Assembly.FullName);
                    }
                ));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
