using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;

namespace ThunderbirdsBoardGameEngine.GameState.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameStateClients(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<GameStateClientOptions>()
                .Bind(configuration.GetSection("GameStateClient"));


            Action<IServiceProvider, HttpClient> configureBase = (sp, http) =>
            {
                var opts = sp
                    .GetRequiredService<IOptions<GameStateClientOptions>>()
                    .Value;

                http.BaseAddress = new Uri(opts.BaseAddress);
            };

            services.AddGameStateV1Clients(configureBase);

            services.AddClientInfrastructure();

            return services;
        }
    }
}
