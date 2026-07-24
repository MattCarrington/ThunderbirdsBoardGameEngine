using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Rescue
{
    public class CardBonusModifierSourceRegistryTests
    {
        [Fact]
        public void TryGetEventCard_WhenSourceExists_ReturnsSource()
        {
            // Arrange
            var source = new TestSource(new CardCode("test-card"));
            var registry = new CardBonusModifierSourceRegistry([source]);

            // Act
            var found = registry.TryGetCard(source.CardCode, out var result);

            // Assert
            Assert.True(found);
            Assert.Same(source, result);
        }

        [Fact]
        public void TryGetEventCard_WhenSourceDoesNotExist_ReturnsFalseAndNull()
        {
            // Arrange
            var registry = new CardBonusModifierSourceRegistry([]);

            // Act
            var found = registry.TryGetCard(new CardCode("missing"), out var result);

            // Assert
            Assert.False(found);
            Assert.Null(result);
        }

        [Fact]
        public void Constructor_WhenSourcesContainNull_ThrowsArgumentException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new CardBonusModifierSourceRegistry([null!]));
        }

        [Fact]
        public void Constructor_WhenSourcesNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CardBonusModifierSourceRegistry(null!));
        }

        private sealed class TestSource : ICardRescueModifierSource
        {
            public TestSource(CardCode eventCardCode)
            {
                CardCode = eventCardCode;
            }

            public CardCode CardCode { get; }

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
