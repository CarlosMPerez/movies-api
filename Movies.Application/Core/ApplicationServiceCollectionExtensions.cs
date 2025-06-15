using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Application.Validators;

namespace Movies.Application.Core;

/// <summary>
/// Extension methods for configuring application layer services in the dependency injection container.
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Adds services from the application layer to the provided <see cref="IServiceCollection"/>.
    /// This method should be called during application startup to register dependencies needed by the application logic.
    /// </summary>
    /// <param name="services">The service collection to which application services are added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> with application services registered.</returns>
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // Ensure the services collection is not null to avoid runtime errors.
        ArgumentNullException.ThrowIfNull(services);

        // Register the MovieRepository as a singleton implementation of IMovieRepository.
        // Singleton means a single instance is created and shared throughout the application's lifetime.
        // This is appropriate if the repository is stateless or manages its own internal state safely.
        services.AddSingleton<IMovieRepository, MovieRepository>();
        services.AddSingleton<IMovieService, MovieService>();

        // Register the validation service as a singleton
        // We could use any part of the assembly but we create a marker interface
        // kind of like a bookmark just for this
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);

        // Return the service collection to allow for method chaining.
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services,
                                                string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ =>
                new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>();
        return services;
    }
}
