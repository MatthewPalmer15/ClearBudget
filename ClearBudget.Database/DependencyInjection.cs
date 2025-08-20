using Microsoft.Extensions.DependencyInjection;

namespace ClearBudget.Database;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>();
        services.AddScoped<IDbContext, ApplicationDbContext>();
        return services;
    }
}