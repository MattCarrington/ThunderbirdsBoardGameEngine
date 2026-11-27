using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Calculation;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Modifiers;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Registries;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Rescue
{
    public class FabCardRescueModifierSourceRegistryTests
    {
        [Fact]
        public void TryGetCard_WhenSourceExists_ReturnsSource()
        {
            // Arrange
            var source = new List<TestSource>
            {
                new(new CardCode("test-card-1"), RescueType.Air),
                new(new CardCode("test-card-2"), RescueType.Land),
                new(new CardCode("test-card-3"), RescueType.Sea)
            };

            var registry = new FabCardRescueModifierSourceRegistry(source);

            // Act
            var found = registry.TryGetCard(RescueType.Air, out var result);

            // Assert
            Assert.True(found);
            Assert.Same(source.First(), result);
        }

        [Fact]
        public void TryGetCard_WhenSourceDoesNotExist_ReturnsFalseAndNull()
        {
            // Arrange
            var registry = new FabCardRescueModifierSourceRegistry([]);

            // Act
            var found = registry.TryGetCard(RescueType.Air, out var result);

            // Assert
            Assert.False(found);
            Assert.Null(result);
        }

        [Fact]
        public void Constructor_WhenSourcesContainNull_ThrowsArgumentException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new FabCardRescueModifierSourceRegistry([null!]));
        }

        [Fact]
        public void Constructor_WhenSourcesNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new FabCardRescueModifierSourceRegistry(null!));
        }

        private sealed class TestSource : IFabCardRescueModifierSource
        {
            public TestSource(CardCode cardCode, RescueType rescueType)
            {
                CardCode = cardCode;
                RescueType = rescueType;
            }

            public CardCode CardCode { get; }

            public RescueType RescueType { get; }

            public int BonusValue => 3;

            public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
            {
                yield return new AppliedRescueModifier
                {
                    Key = "Test Source",
                    Value = 2,
                    SourceType = SourceType.EventCard
                };
            }
        }
    }
}
