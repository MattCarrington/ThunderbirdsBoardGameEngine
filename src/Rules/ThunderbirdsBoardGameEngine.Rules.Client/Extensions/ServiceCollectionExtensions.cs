using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.Rules.Client.Configuration;

namespace ThunderbirdsBoardGameEngine.Rules.Client.Extensions
{
    /// <summary>
    /// Provides extension methods for registering Rules client services and related configuration with an <see
    /// cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>These extension methods are intended to simplify the setup of Rules client dependencies in an
    /// application's dependency injection container. They configure options, validation, and required HTTP clients for
    /// consuming Rules APIs.</remarks>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures Rules client services and related dependencies to the specified service collection.
        /// </summary>
        /// <remarks>This method registers the Rules client options, validation, and HTTP client
        /// configuration using settings from the provided configuration. It also adds required infrastructure and
        /// versioned client services. Call this method during application startup to enable Rules client
        /// functionality.</remarks>
        /// <param name="services">The service collection to which the Rules client services will be added. Must not be null.</param>
        /// <param name="configuration">The application configuration used to bind Rules client options. Must not be null.</param>
        /// <returns>The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
        public static IServiceCollection AddRulesClients(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<RulesClientOptions>()
                .Bind(configuration.GetSection("RulesClient"))
                .ValidateOnStart();

            services.AddSingleton<
                IPostConfigureOptions<RulesClientOptions>,
                RulesClientOptionsPostConfigure>();

            services.AddSingleton<
                IValidateOptions<RulesClientOptions>,
                RulesClientOptionsValidator>();

            Action<IServiceProvider, HttpClient> configureBase = (sp, http) =>
            {
                var opts = sp
                    .GetRequiredService<IOptions<RulesClientOptions>>()
                    .Value;

                http.BaseAddress = new Uri(opts.BaseAddress);
            };

            services.AddRulesV1Clients(configureBase);

            services.AddClientInfrastructure();

            return services;
        }
    }
}
