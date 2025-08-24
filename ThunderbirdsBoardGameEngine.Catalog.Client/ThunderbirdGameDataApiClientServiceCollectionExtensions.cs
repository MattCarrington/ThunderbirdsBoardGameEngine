using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Client.Clients.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;

namespace ThunderbirdsBoardGameEngine.Catalog.Client
{
    public static class ClientServiceCollectionExtensions
    {
        public static IServiceCollection AddGameDataClients(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("GameDataClient");
            services.Configure<GameDataClientOptions>(section);

            var options = section.Get<GameDataClientOptions>()!;

            services.AddHttpClient<IDisasterCardClient, DisasterCardClient>(client =>
            {
                client.BaseAddress = new Uri(options.BaseAddress);
            });

            return services;
        }
    }
}
