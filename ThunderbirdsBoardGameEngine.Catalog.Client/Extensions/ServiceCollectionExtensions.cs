using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCatalogClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<CatalogClientOptions>()
                .Bind(configuration.GetSection("CatalogClient")) // keep your current section name
                .Validate(o => !string.IsNullOrWhiteSpace(o.BaseAddress), "CatalogClient:BaseAddress required")
                .Validate(o => Uri.TryCreate(o.BaseAddress, UriKind.Absolute, out _), "CatalogClient.BaseAddress must be an absolute URI")
                .ValidateOnStart();

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
