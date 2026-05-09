using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Application.Services;

namespace ThunderbirdsBoardGameEngine.Catalog.Application
{
    /// <summary>
    /// Extension methods for registering Catalog application-layer services.
    /// </summary>
    /// <remarks>
    /// This method wires up the application-layer abstractions and services
    /// for the Catalog bounded context.
    ///
    /// It does not register infrastructure concerns such as persistence or
    /// external integrations.
    /// </remarks>
    [Obsolete("Catalog API is deprecated. Use Reference Data instead")]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the Catalog application services.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> to register services with.
        /// </param>
        /// <returns>
        /// The same <see cref="IServiceCollection"/> instance for chaining.
        /// </returns>
        public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
        {
            services.AddSingleton<IDisasterCardService, DisasterCardService>();
            services.AddSingleton<ICharacterDefinitionService, CharacterDefinitionService>();
            return services;
        }
    }
}
