using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Speed;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Movement
{
    public class MovementSpeedModifierSourceRegistryTests
    {
        private static CardCode TestCardCode1 => new("test-1");

        private static CardCode TestCardCode2 => new("test-2");

        [Fact]
        public void TryGet_WhenSourceExists_ShouldReturnTrueAndSource()
        {
            // Arrange
            var registry = CreateRegistry();

            var cardCode = TestCardCode2;

            // Act
            var result = registry.TryGetEventCard(cardCode, out var source);

            // Assert
            Assert.True(result);
            Assert.NotNull(source);
            Assert.Equal(cardCode, source.EventCardCode);
        }

        [Fact]
        public void TryGet_WhenSourceDoesNotExist_ShouldReturnFalseAndNull()
        {
            // Arrange
            var registry = CreateRegistry();

            var cardCode = new CardCode("non-existent-card");

            // Act
            var result = registry.TryGetEventCard(cardCode, out var source);

            // Assert
            Assert.False(result);
            Assert.Null(source);
        }

        [Fact]
        public void Constructor_WhenSourcesContainNull_ShouldThrowArgumentException()
        {
            // Arrange
            var sources = new List<IMovementSpeedModifierSource>
            {
                new TestRegistrySource1(),
                null!, // Intentionally adding a null source
                new TestRegistrySource2(),
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new MovementSpeedModifierSourceRegistry(sources));
        }

        [Fact]
        public void Constructor_WhenSourcesNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            List<IMovementSpeedModifierSource> sources = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MovementSpeedModifierSourceRegistry(sources));
        }

        private static MovementSpeedModifierSourceRegistry CreateRegistry()
        {
            var sources = new List<IMovementSpeedModifierSource>
            {
                new TestRegistrySource1(),
                new TestRegistrySource2(),
            };

            return new MovementSpeedModifierSourceRegistry(sources);
        }

        private class TestRegistrySource1 : IMovementSpeedModifierSource
        {
            public CardCode EventCardCode => TestCardCode1;

            public AppliedMovementSpeedModifier? ApplyMovementModifier(ThunderbirdCode input)
            {
                return new AppliedMovementSpeedModifier(
                    Card: EventCardCode,
                    EffectiveTopSpeed: 100,
                    Message: $"{EventCardCode} applied");
            }
        }

        private class TestRegistrySource2 : IMovementSpeedModifierSource
        {
            public CardCode EventCardCode => TestCardCode2;

            public AppliedMovementSpeedModifier? ApplyMovementModifier(ThunderbirdCode input)
            {
                return new AppliedMovementSpeedModifier(
                    Card: EventCardCode,
                    EffectiveTopSpeed: 1,
                    Message: $"{EventCardCode} applied");
            }
        }
    }
}
