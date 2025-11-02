using ChallengeApp.Application.Elastic;
using ChallengeApp.Domain.Primitives;
using ChallengeApp.Domain.Repositories;
using ChallengeApp.Infrastructure.Data;
using ChallengeApp.Infrastructure.Data.Interceptors;
using ChallengeApp.Infrastructure.Elastic;
using ChallengeApp.Infrastructure.EventBuses;
using ChallengeApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChallengeApp.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString);
        });
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddSingleton<IEventBus, KafkaEventBus>();

        services.AddScoped(typeof(IElasticsearchService<>), typeof(ElasticsearchService<>));
        
        return services;
    }
}