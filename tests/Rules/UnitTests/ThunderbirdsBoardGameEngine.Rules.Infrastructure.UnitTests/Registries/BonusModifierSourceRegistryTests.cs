using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.EventCards;
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
        [InlineData("the-hood-interferes", typeof(TheHoodInterferes))]
        public void GetBonusModifierSource_WhenCardExistsInRegistry_ShouldReturnExpectedSource(string cardCodeString, Type expectedType)
        {
            // Arrange
            var registry = new BonusModifierSourceRegistry();

            var cardCode = new CardCode(cardCodeString);

            // Act
            var source = registry.TryGetBonusModifierSource(cardCode, out var result);

            // Assert
            Assert.True(source);
            Assert.NotNull(result);
            Assert.IsType(expectedType, result);
        }

        [Fact]
        public void GetBonusModifierSource_WhenCardDoesNotExistInRegistry_ShouldReturnNull()
        {
            // Arrange
            var registry = new BonusModifierSourceRegistry();

            var cardCode = new CardCode("non-existent-card");

            // Act
            var source = registry.TryGetBonusModifierSource(cardCode, out var result);

            // Assert
            Assert.False(source);
            Assert.Null(result);
        }
    }
}
