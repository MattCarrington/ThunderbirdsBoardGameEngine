using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.GameState.Client.Extensions;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Pages;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Services;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Support;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.StepDefinitions
{
    public static class TestDependencies
    {
        [ScenarioDependencies]
        public static IServiceCollection CreateServices()
        {
            var baseUrl =
                Environment.GetEnvironmentVariable("BASE_URL")
                ?? throw new InvalidOperationException(
                    "Environment variable BASE_URL is not set.");

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["GameStateClient:BaseAddress"] = baseUrl
                    })
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<TestSettings>();

            services.AddScoped<UiContext>();
            services.AddScoped<PlaywrightTestContext>();

            services.AddGameStateClients(configuration);

            services.AddScoped<GamePrerequisiteService>();

            services.AddScoped<HomePage>();
            services.AddScoped<GameDashboardPage>();
            services.AddScoped<MovementPage>();
            services.AddScoped<DisasterCardsPage>();

            return services;
        }
    }
}
