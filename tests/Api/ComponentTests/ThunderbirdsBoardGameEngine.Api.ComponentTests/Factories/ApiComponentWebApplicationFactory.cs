using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.TestUtils.GameState;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories
{
    public class ApiComponentWebApplicationFactory : WebApplicationFactory<Program>
    {
        public InMemoryGameRepository Repository { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting(
        "GameState:Persistence:ConnectionString",
        "Host=localhost;Database=unused;Username=unused;Password=unused");

            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            });

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IGameRepository>();

                services.AddSingleton(Repository);
                services.AddSingleton<IGameRepository>(provider =>
                    provider.GetRequiredService<InMemoryGameRepository>());
            });
        }
    }
}
