using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.GameStateIntegrity;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameState(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<GameStatePersistenceOptions>()
                .Bind(configuration.GetSection(GameStatePersistenceOptions.SectionName));

            services.AddMediatR(typeof(CreateNewGameHandler).Assembly);

            services.AddSingleton<IGameStateIntegrityValidator, ReferenceDataGameStateIntegrityValidator>();

            services.AddDbContext<GameStateDbContext>((serviceProvider, options) =>
            {
                var persistenceOptions = serviceProvider.GetRequiredService<IOptions<GameStatePersistenceOptions>>().Value;
                options.UseNpgsql(persistenceOptions.ConnectionString);
            });

            services.AddScoped<IGameRepository, GameRepository>();
            services.AddSingleton<GameRecordMapper>();

            RegisterCreateGame(services);

            return services;
        }

        private static void RegisterCreateGame(IServiceCollection services)
        {
            services.AddSingleton<StandardGameSetupFactory>();
        }
    }
}
