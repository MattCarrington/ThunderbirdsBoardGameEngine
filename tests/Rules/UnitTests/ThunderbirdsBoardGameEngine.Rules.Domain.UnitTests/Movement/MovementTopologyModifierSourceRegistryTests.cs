using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Movement
{
    public class MovementTopologyModifierSourceRegistryTests
    {
        [Fact]
        public void TryGetEventCard_WhenSourceExists_ReturnsSource()
        {
            // Arrange
            var source = new TestSource(new CardCode("test-card"));
            var registry = new MovementTopologyModifierSourceRegistry([source]);

            // Act
            var found = registry.TryGetEventCard(source.CardCode, out var result);

            // Assert
            Assert.True(found);
            Assert.Same(source, result);
        }

        [Fact]
        public void TryGetEventCard_WhenSourceDoesNotExist_ReturnsFalseAndNull()
        {
            // Arrange
            var registry = new MovementTopologyModifierSourceRegistry([]);

            // Act
            var found = registry.TryGetEventCard(new CardCode("missing"), out var result);

            // Assert
            Assert.False(found);
            Assert.Null(result);
        }

        [Fact]
        public void Constructor_WhenSourcesContainNull_ThrowsArgumentException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new MovementTopologyModifierSourceRegistry([null!]));
        }

        [Fact]
        public void Constructor_WhenSourcesNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MovementTopologyModifierSourceRegistry(null!));
        }

        private sealed class TestSource : IMovementTopologyModifierSource
        {
            public TestSource(CardCode eventCardCode)
            {
                CardCode = eventCardCode;
            }

            public CardCode CardCode { get; }

            public AppliedMovementTopologyModifier ApplyMovementModifier()
            {
                return new AppliedMovementTopologyModifier(CardCode, [], "Test");
            }
        }
    }
}
