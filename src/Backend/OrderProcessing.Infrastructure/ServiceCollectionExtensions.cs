using Microsoft.Extensions.DependencyInjection;
using OrderProcessing.Domain.Interfaces;
using OrderProcessing.Domain.Interfaces.Repositories;
using OrderProcessing.Infrastructure.Data;
using OrderProcessing.Infrastructure.Repositories;

namespace OrderProcessing.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // Register the Postgres connection factory
        services.AddSingleton<IDbConnectionFactory>(_ => new PostgresConnectionFactory(connectionString));

        // Register repositories
        services.AddScoped<IBookRepository, BookRepository>();

        return services;
    }
}
