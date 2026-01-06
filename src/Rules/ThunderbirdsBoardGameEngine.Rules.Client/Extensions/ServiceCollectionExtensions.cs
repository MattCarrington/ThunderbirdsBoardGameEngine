using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.Rules.Client.Configuration;

namespace ThunderbirdsBoardGameEngine.Rules.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
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
