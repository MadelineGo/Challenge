using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ChallengeApp.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        return services;
    }
}