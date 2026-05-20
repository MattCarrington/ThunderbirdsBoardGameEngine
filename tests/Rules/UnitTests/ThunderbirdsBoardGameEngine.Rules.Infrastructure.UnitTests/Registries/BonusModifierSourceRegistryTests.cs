using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.FabCards;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure.Registries;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.UnitTests.Registries
{
    public class BonusModifierSourceRegistryTests
    {
        [Theory]
        [InlineData("remote-control-hover-camera", typeof(RemoteControlHoverCamera))]
        [InlineData("personal-hoverjet", typeof(PersonalHoverjet))]
        [InlineData("underwater-sealing-unit", typeof(UnderwaterSealingUnit))]
        [InlineData("astronaut-spacewalk", typeof(AstronautSpacewalk))]
        public void GetBonusModifierSource_WhenCardExistsInRegistry_ShouldReturnExpectedSource(string cardCodeString, Type expectedType)
        {
            // Arrange
            var registry = new BonusModifierSourceRegistry();

            var cardCode = new CardCode(cardCodeString);

            // Act
            var source = registry.GetBonusModifierSource(cardCode);

            // Assert
            Assert.NotNull(source);
            Assert.IsType(expectedType, source);
        }

        [Fact]
        public void GetBonusModifierSource_WhenCardDoesNotExistInRegistry_ShouldReturnNull()
        {
            // Arrange
            var registry = new BonusModifierSourceRegistry();

            var cardCode = new CardCode("non-existent-card");

            // Act
            var source = registry.GetBonusModifierSource(cardCode);

            // Assert
            Assert.Null(source);
        }
    }
}
