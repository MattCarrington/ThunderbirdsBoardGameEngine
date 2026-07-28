using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.GameStateIntegrity;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Configuration;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameStatePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<GameStatePersistenceOptions>()
                .Bind(configuration.GetSection(
                    GameStatePersistenceOptions.SectionName))
                .ValidateOnStart();

            services.AddSingleton<
                IValidateOptions<GameStatePersistenceOptions>,
                GameStatePersistenceOptionsValidator>();

            services.AddDbContext<GameStateDbContext>((serviceProvider, options) =>
            {
                var persistenceOptions = serviceProvider.GetRequiredService<IOptions<GameStatePersistenceOptions>>().Value;
                options.UseNpgsql(persistenceOptions.ConnectionString);
            });

            services.AddScoped<IGameRepository, GameRepository>();
            services.AddSingleton<GameRecordMapper>();

            return services;
        }

        public static IServiceCollection AddGameStateIntegrity(this IServiceCollection services)
        {
            services.AddSingleton<IGameStateIntegrityValidator, ReferenceDataGameStateIntegrityValidator>();

            return services;
        }
    }
}
