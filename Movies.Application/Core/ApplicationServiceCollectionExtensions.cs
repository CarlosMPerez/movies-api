using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Repositories;

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

        // Return the service collection to allow for method chaining.
        return services;    }
}
