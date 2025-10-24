using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Api.Startup;
using ThunderbirdsBoardGameEngine.Catalog.Application.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Application.Validators;

namespace ThunderbirdsBoardGameEngine.Api.Composition
{
    public static class WarmupServiceCollectionExtensions
    {
        public static IServiceCollection AddCatalogWarmupServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<DisasterCardWarmupOptions>()
                .Bind(configuration.GetSection("Catalog:DisasterCards:Warmup"))
                .ValidateOnStart();

            services.AddSingleton<IValidateOptions<DisasterCardWarmupOptions>, DisasterCardWarmupOptionsValidator>();
            services.AddHostedService<DisasterCardWarmupHostedService>();

            return services;
        }
    }
}
