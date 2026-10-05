using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Courses.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddCoursesPresentation(this IServiceCollection services)
    {
        services.AddControllersWithViews()
            .AddApplicationPart(Assembly.GetExecutingAssembly());
            
        return services;
    }
}
