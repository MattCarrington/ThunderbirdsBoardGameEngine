using ThunderbirdsBoardGameEngine.Api.Mappers.Rules.V1;
using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Mappers.Rules.V1
{
    public class RescueMappingExtensionsTests
    {
        [Fact]
        public void ToQuery_ValidDto_ReturnsExpectedQuery()
        {
            // Arrange
            var disasterCardCode = new CardCode("card-code-123");

            var dto = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = ["character:alan", "thunderbird:thunderbird4", "podvehicle.domo"],
                PerformingCharacterKey = "alan"
            };

            // Act
            var result = dto.ToQuery(disasterCardCode.ToString());

            // Assert
            Assert.Equal(disasterCardCode, result.DisasterCardCode);
            Assert.Equal(dto.PresentDisasterBonusKeys.Select(k => new DisasterBonusKey(k)), result.RescueCalculationInput.PresentDisasterBonusKeys);
        }

        [Fact]
        public void ToDto_ValidResponse_ReturnsExpectedDto()
        {
            // Arrange
            var response = new CalculateRescueTargetResponse
            (
                TargetNumber: 12,
                TotalBonus: 3,
                AppliedBonuses:
                [
                    new DisasterBonus(new DisasterBonusKey("character:alan"), 2),
                    new DisasterBonus(new DisasterBonusKey("thunderbird:thunderbird4"), 1)
                ]
            );

            // Act
            var result = response.ToDto();

            // Assert
            Assert.Equal(response.TargetNumber, result.TargetNumber);
            Assert.Equal(response.TotalBonus, result.TotalBonus);
            Assert.Equal(response.AppliedBonuses.Count, result.AppliedDisasterBonuses.Count);

            var expectedBonus = new List<AppliedDisasterBonusDto>
            {
                new() { BonusKey = "character:alan", BonusValue = 2, SourceType = "disaster-card" },
                new() { BonusKey = "thunderbird:thunderbird4", BonusValue = 1, SourceType = "disaster-card" }
            };

            Assert.All(result.AppliedDisasterBonuses, expected =>
                Assert.Contains(expected, expectedBonus));
        }
    }
}
