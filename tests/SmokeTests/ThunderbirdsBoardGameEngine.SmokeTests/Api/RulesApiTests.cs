using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.SmokeTests.Factories;
using Xunit;

namespace ThunderbirdsBoardGameEngine.SmokeTests.Api
{
    public class RulesApiTests
    {
        [Fact]
        public async Task RescueEndpointIsReachable()
        {
            // Arrange
            using var sp = RulesClientProviderFactory.Build(SmokeTestConfig.SmokeTestBaseUrl);
            var client = sp.GetRequiredService<IRescueClient>();

            var request = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = [],
                PerformingCharacterKey = "john"
            };

            // Act
            var response = await client.CalculateRescueTargetAsync("the-cham-cham", request, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(response.Success, $"Expected a successful status code, but got {(int)response.StatusCode}");
            Assert.IsType<CalculateRescueTargetResponseDto>(response.Data);
        }
    }
}
