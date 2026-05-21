using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.UnitTests.Lookups
{
    public class CatalogDisasterCatalogLookupTests
    {
        [Fact]
        public void GetDisasterRescueContribution_WhenValidDisasterCard_ReturnsRescueContext()
        {
            // Arrange
            var code = new CardCode("test-disaster-card");

            var disaster = new ReferenceDisasterDefinition(
                code: code,
                displayName: "Test Disaster Card",
                difficultyNumber: 7,
                location: new LocationCode("test-location"),
                rescueType: RescueType.Air,
                bonuses:
                [
                    new ReferenceDisasterBonus(new DisasterBonusKey("character-1"), 1, null),
                    new ReferenceDisasterBonus(new DisasterBonusKey("thunderbird-1"), 2, null),
                    new ReferenceDisasterBonus(new DisasterBonusKey("pod-vehicle-1"), 3, null)
                ],
                rewards:
                [
                    new ReferenceDisasterReward.PlayerChoice(),
                    new ReferenceDisasterReward.SpecificToken(BonusToken.Technology)
                ]
            );

            var catalog = Substitute.For<IDisasterDefinitionCatalog>();
            catalog.GetByCode(Arg.Any<CardCode>()).Returns(disaster);

            var lookup = new ReferenceDisasterCatalogLookup(catalog);

            // Act
            var result = lookup.GetDisasterRescueContribution(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(disaster.RescueType, result.RescueType);
            Assert.Equal(disaster.DifficultyNumber, result.DifficultyNumber);

            var expectedBonuses = new[]
            {
                new DisasterBonus(new("character-1"), 1),
                new DisasterBonus(new("thunderbird-1"), 2),
                new DisasterBonus(new("pod-vehicle-1"), 3)
            };

            Assert.Equal(expectedBonuses.Length, result.AvailableBonuses.Count);
            Assert.All(expectedBonuses, expected =>
                Assert.Contains(expected, result.AvailableBonuses));
        }

        [Fact]
        public void GetDisasterRescueContribution_WhenDisasterCardNotFound_ThrowsReferenceDataNotFoundException()
        {
            // Arrange
            var code = new CardCode("non-existent-disaster-card");

            var catalog = Substitute.For<IDisasterDefinitionCatalog>();
            catalog.GetByCode(Arg.Any<CardCode>()).Throws(new KeyNotFoundException());

            var lookup = new ReferenceDisasterCatalogLookup(catalog);

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataNotFoundException>(() => lookup.GetDisasterRescueContribution(code));
            Assert.Equal("Disaster", ex.ResourceType);
            Assert.Equal(code.ToString(), ex.Code);
        }
    }
}
