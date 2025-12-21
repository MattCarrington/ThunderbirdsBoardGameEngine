using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Configuration;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Extensions
{
    /// <summary>
    /// Service registration for Catalog SDK clients.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Catalog SDK clients and configures the shared HTTP pipeline.
        /// </summary>
        /// <param name="services">The DI container.</param>
        /// <param name="configuration">App configuration containing the <c>CatalogClient</c> section.</param>
        /// <remarks>
        /// Requires <c>CatalogClient:BaseAddress</c> to be a valid absolute URI.
        /// Use <see cref="CatalogClientsV1Registration"/> to add version-specific clients.
        /// </remarks>
        public static IServiceCollection AddCatalogClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<CatalogClientOptions>()
                .Bind(configuration.GetSection("CatalogClient"))
                .ValidateOnStart();

            services.AddSingleton<IPostConfigureOptions<CatalogClientOptions>, CatalogClientOptionsPostConfigure>();
            services.AddSingleton<IValidateOptions<CatalogClientOptions>, CatalogClientOptionsValidator>();

            // Shared HttpClient configuration for ALL Catalog typed clients
            Action<IServiceProvider, HttpClient> configureBase = (sp, http) =>
            {
                var opts = sp.GetRequiredService<IOptions<CatalogClientOptions>>().Value;
                http.BaseAddress = new Uri(opts.BaseAddress);
                // optional: http.Timeout = TimeSpan.FromSeconds(30);
                // optional: http.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            };

            // Register the V1 pipeline & clients (can add V2 later without touching this method)
            services.AddCatalogV1Clients(configureBase);

            return services;
        }
    }
}
