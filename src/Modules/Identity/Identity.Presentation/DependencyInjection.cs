using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Identity.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityPresentation(this IServiceCollection services)
    {
        services.AddControllersWithViews()
            .AddApplicationPart(Assembly.GetExecutingAssembly());
            
        return services;
    }
}
