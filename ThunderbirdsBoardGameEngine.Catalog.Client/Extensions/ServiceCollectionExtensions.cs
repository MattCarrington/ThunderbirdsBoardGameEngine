using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Client.Clients.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCatalogClients(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("CatalogClient");

            services.AddOptions<CatalogClientOptions>()
                    .Bind(section)
                    .Validate(o => !string.IsNullOrWhiteSpace(o.BaseAddress), "CatalogClient:BaseAddress required")
                    .Validate(o => Uri.TryCreate(o.BaseAddress, UriKind.Absolute, out _), "CatalogClient.BaseAddress must be an absolute URI")
                    .ValidateOnStart();

            services.AddHttpClient<IDisasterCardsClient, DisasterCardsClient>((sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<CatalogClientOptions>>().Value;
                client.BaseAddress = new Uri(opts.BaseAddress);
            });

            return services;
        }
    }
}
