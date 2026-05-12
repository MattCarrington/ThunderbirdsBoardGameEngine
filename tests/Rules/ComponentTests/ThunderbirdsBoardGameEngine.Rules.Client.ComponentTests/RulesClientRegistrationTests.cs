using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Rules.Client.Configuration;
using ThunderbirdsBoardGameEngine.TestUtils.Rules.Factories;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Client.ComponentTests
{
    public class RulesClientRegistrationTests
    {
        [Fact]
        public async Task AddRulesClients_WhenInvalidBaseAddress_ThrowsException()
        {
            // Arrange
            await using var provider = RulesClientProviderFactory.Build("http:/example.com");

            // Act & Assert
            Assert.Throws<OptionsValidationException>(
                () => provider.GetRequiredService<IOptions<RulesClientOptions>>().Value);
        }
    }
}
