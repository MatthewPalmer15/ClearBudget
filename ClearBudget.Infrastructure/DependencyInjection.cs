using ClearBudget.Infrastructure.Encryption;
using ClearBudget.Infrastructure.MediatR.Pipelines;
using ClearBudget.Infrastructure.Services.Caching;
using ClearBudget.Infrastructure.Services.Cookie;
using ClearBudget.Infrastructure.Services.Csv;
using ClearBudget.Infrastructure.Services.Hash;
using ClearBudget.Infrastructure.Services.Serialization;
using ClearBudget.Infrastructure.Services.Session;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClearBudget.Infrastructure;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachedRequestBehaviour<,>));

        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<ICsvService, CsvService>();

        services.AddMemoryCache();
        services.AddDistributedMemoryCache();

        services.AddScoped<ICacheManager, CacheManager>();

        services.AddScoped<IJsonSerializer, JsonSerializer>();
        services.AddScoped<IXmlSerializer, XmlSerializer>();

        services.AddScoped<ICookieManager, CookieManager>();
        services.AddScoped<ISessionManager, SessionManager>();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(60);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        services.AddScoped<IHashService, HashService>();


        var config = TypeAdapterConfig.GlobalSettings;

        services.AddSingleton(config);
        services.AddScoped<IMapper, Mapper>();
        return services;
    }
}