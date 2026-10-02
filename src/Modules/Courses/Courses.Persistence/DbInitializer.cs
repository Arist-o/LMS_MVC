using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Courses.Persistence.Context;
namespace Courses.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();

            await context.Database.MigrateAsync();

            if (!await context.Courses.AnyAsync())
            {
                // Seed initial data
            }

            if (!await context.Lessons.AnyAsync())
            {
                // Seed initial data for lessons
            }

            if(!await context.Tests.AnyAsync())
            {
                // Seed initial data for tests
            }

            if(!await context.Reviews.AnyAsync())
            {
                // Seed initial data for reviews
            }

        }
    }
}
