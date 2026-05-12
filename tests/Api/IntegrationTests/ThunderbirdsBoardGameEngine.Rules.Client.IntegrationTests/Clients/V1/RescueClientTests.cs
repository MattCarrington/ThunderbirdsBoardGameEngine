using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Rules.Factories;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Client.IntegrationTests.Clients.V1
{
    public class RescueClientTests
    {
        [Fact]
        public async Task CalculateRescueTarget_ReturnsResult()
        {
            // Arrange
            using var sp = RulesClientProviderFactory.Build(RulesTestConfig.RulesBaseUrl);
            var client = sp.GetRequiredService<IRescueClient>();

            var request = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = ["virgil", "thunderbird-4"],
                PerformingCharacterKey = "john"
            };

            var code = "terror-in-new-york-city";

            // Act
            var response = await client.CalculateRescueTargetAsync(code, request);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Null(response.ErrorMessage);

            var result = Assert.IsType<CalculateRescueTargetResponseDto>(response.Data, exactMatch: false);
            Assert.NotNull(result);
            Assert.Equal(7, result.TargetNumber);
            Assert.Equal(4, result.TotalBonus);

            Assert.IsType<IReadOnlyCollection<AppliedDisasterBonusDto>>(result.AppliedDisasterBonuses, exactMatch: false);
            Assert.Equal(2, result.AppliedDisasterBonuses.Count);
            Assert.Contains(result.AppliedDisasterBonuses, b => b.BonusKey == "virgil" && b.BonusValue == 2 && b.SourceType == "disaster-card");
            Assert.Contains(result.AppliedDisasterBonuses, b => b.BonusKey == "thunderbird-4" && b.BonusValue == 2 && b.SourceType == "disaster-card");
            Assert.DoesNotContain(result.AppliedDisasterBonuses, b => b.BonusKey == "firefly");
            Assert.DoesNotContain(result.AppliedDisasterBonuses, b => b.BonusKey == "john");
            Assert.DoesNotContain(result.AppliedDisasterBonuses, b => b.SourceType == "character-ability");
        }
    }
}
