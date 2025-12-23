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
            var dto = new CalculateRescueTargetRequestDto
            {
                CardId = 5,
                AppliedBonusKeys = ["character:alan", "thunderbird:thunderbird4", "podvehicle.domo"]
            };

            // Act
            var result = dto.ToQuery();

            // Assert
            Assert.Equal(dto.CardId, result.DisasterCardId);
            Assert.Equal(dto.AppliedBonusKeys.Select(k => new DisasterBonusKey(k)), result.AppliedBonusKeys);
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
            Assert.Equal(response.AppliedBonuses.Count, result.AppliedBonuses.Count);

            var expectedBonus = new List<AppliedBonusDto>
            {
                new() { BonusKey = "character:alan", BonusValue = 2 },
                new() { BonusKey = "thunderbird:thunderbird4", BonusValue = 1 }
            };

            Assert.All(result.AppliedBonuses, expected =>
                Assert.Contains(expected, expectedBonus));
        }
    }
}
