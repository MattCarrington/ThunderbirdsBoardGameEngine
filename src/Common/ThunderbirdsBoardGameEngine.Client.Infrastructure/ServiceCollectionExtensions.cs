using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.Handlers;

namespace ThunderbirdsBoardGameEngine.Client.Infrastructure
{
    /// <summary>
    /// Provides extension methods for registering client infrastructure services with an <see
    /// cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds client infrastructure services required for HTTP response handling to the specified service collection.
        /// </summary>
        /// <param name="services">The service collection to which the client infrastructure services will be added. Cannot be null.</param>
        /// <returns>The same instance of <see cref="IServiceCollection"/> that was provided, to support method chaining.</returns>
        public static IServiceCollection AddClientInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IHttpResponseHandler, DefaultHttpResponseHandler>();
            return services;
        }
    }
}
