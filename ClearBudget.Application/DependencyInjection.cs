using ClearBudget.Application.Services;
using ClearBudget.Database;
using ClearBudget.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClearBudget.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddDatabase();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // services.AddHostedService<RecurringTransactionsService>(); TODO: TEST THIS BEFORE ADDING INTO MAIN
        return services;
    }
}
