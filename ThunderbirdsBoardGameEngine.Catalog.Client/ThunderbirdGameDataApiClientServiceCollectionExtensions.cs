using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Client.Clients.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;

namespace ThunderbirdsBoardGameEngine.Catalog.Client
{
    public static class ClientServiceCollectionExtensions
    {
        public static IServiceCollection AddGameDataClients(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("GameDataClient");

            services.AddOptions<GameDataClientOptions>()
                    .Bind(section)
                    .Validate(o => Uri.TryCreate(o.BaseAddress, UriKind.Absolute, out _),
                              "GameDataClient.BaseAddress must be an absolute URI");

            services.AddHttpClient<IDisasterCardsClient, DisasterCardsClient>((sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<GameDataClientOptions>>().Value;
                client.BaseAddress = new Uri(opts.BaseAddress);
            });

            return services;
        }
    }
}
