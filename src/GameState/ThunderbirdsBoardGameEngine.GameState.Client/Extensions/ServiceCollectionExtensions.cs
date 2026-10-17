using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.Client.Core.Configuration;
using ThunderbirdsBoardGameEngine.GameState.Client.Configuration;

namespace ThunderbirdsBoardGameEngine.GameState.Client.Extensions
{
    /// <summary>
    /// Contains extension methods for registering game state HTTP clients and their configuration with an
    /// IServiceCollection.
    /// </summary>
    /// <remarks>Binds GameStateClientOptions from configuration, registers post-configure and validation
    /// services, configures HttpClient BaseAddress from the bound options, adds versioned game-state clients and shared
    /// client infrastructure, and returns the IServiceCollection for chaining.</remarks>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers and configures GameState HTTP API clients and related services in the provided IServiceCollection.
        /// </summary>
        /// <remarks>Binds GameStateClientOptions and validates them on startup, registers post-configure
        /// and validator implementations, configures HttpClient BaseAddress from options, and adds versioned GameState
        /// clients and shared client infrastructure.</remarks>
        /// <param name="services">The IServiceCollection to which clients, options, validators, and infrastructure are added.</param>
        /// <param name="configuration">The IConfiguration used to bind GameStateClientOptions from the configured section.</param>
        /// <returns>The same IServiceCollection instance to allow method chaining.</returns>
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
