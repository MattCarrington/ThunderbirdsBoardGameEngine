using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.GameState.Client.Configuration;
using ThunderbirdsBoardGameEngine.GameState.Client.Extensions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Client.ComponentTests
{
    public class GameStateClientRegistrationTests
    {
        [Fact]
        public void AddGameStateClients_WhenInvalidBaseAddress_ThrowsException()
        {
            // Arrange
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["RulesClient:BaseAddress"] = "http:/example.com"
                })
                .Build();

            var services = new ServiceCollection();
            services.AddGameStateClients(cfg);

            var provider = services.BuildServiceProvider();

            // Act & Assert
            Assert.Throws<OptionsValidationException>(
                () => provider.GetRequiredService<IOptions<GameStateClientOptions>>().Value);
        }
    }
}
