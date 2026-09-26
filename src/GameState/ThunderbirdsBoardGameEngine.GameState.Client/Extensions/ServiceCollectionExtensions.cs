using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.Client.Core.Configuration;
using ThunderbirdsBoardGameEngine.GameState.Client.Configuration;

namespace ThunderbirdsBoardGameEngine.GameState.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameStateClients(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<GameStateClientOptions>()
                .Bind(configuration.GetSection(GameStateClientOptions.SectionName))
                .ValidateOnStart();

            services.AddSingleton<
                IPostConfigureOptions<GameStateClientOptions>,
                ApiClientOptionsPostConfigure<GameStateClientOptions>>();

            services.AddSingleton<
                IValidateOptions<GameStateClientOptions>,
                ApiClientOptionsValidator<GameStateClientOptions>>();

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
