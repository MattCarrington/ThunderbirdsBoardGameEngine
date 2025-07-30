using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.GameData.Api.Client.Clients.V1;
using ThunderbirdsBoardGameEngine.GameData.Api.Client.Interfaces.V1;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Client
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
