using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ReferenceSources;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.ReferenceSources
{
    public class InMemoryDisasterCardReferenceSourceTests
    {
        private readonly ImmutableArray<DisasterCard> _cards =
        [
            new DisasterCardBuilder().WithId(1).WithName("Test 1").Build(),
            new DisasterCardBuilder().WithId(2).WithName("Test 2").Build()
        ];

        private const string _version = "1.0";

        [Fact]
        public void Constructor_WithValidParameters_SetsProperties()
        {
            // Arrange          

            // Act
            var source = CreateReferenceSource(_cards, _version);

            // Assert
            Assert.Equal(_version, source.Version);
            Assert.Equal(2, source.Cards.Length);
            Assert.Contains(source.Cards, c => c.Id == 1 && c.Name == "Test 1");
            Assert.Contains(source.Cards, c => c.Id == 2 && c.Name == "Test 2");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidVersion_ThrowsArgumentException(string invalidVersion)
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new InMemoryDisasterCardReferenceSource(_cards, invalidVersion));
        }

        [Fact]
        public void Constructor_WithNullVersion_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new InMemoryDisasterCardReferenceSource(_cards, null!));
        }

        [Fact]
        public void Constructor_WithEmptyCards_ThrowsArgumentException()
        {
            // Arrange
            var emptyCards = ImmutableArray<DisasterCard>.Empty;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new InMemoryDisasterCardReferenceSource(emptyCards, _version));
        }

        [Fact]
        public void All_WhenChanged_IsImmutable()
        {
            // Arrange
            var cards = _cards.ToList();

            var source = CreateReferenceSource(cards);

            // Act
            cards.Add(new DisasterCardBuilder().WithId(3).WithName("Test 3").Build());

            // Assert
            Assert.Equal(2, source.Cards.Length);
        }

        [Fact]
        public void GetById_WithExistingId_ReturnsCorrectCard()
        {
            // Arrange
            var source = CreateReferenceSource(_cards);

            // Act
            var card = source.GetById(1);

            // Assert
            Assert.NotNull(card);
            Assert.Equal(1, card.Id);
            Assert.Equal("Test 1", card.Name);
        }

        [Fact]
        public void GetById_WithNonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var source = CreateReferenceSource(_cards);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => source.GetById(999));
        }

        private static InMemoryDisasterCardReferenceSource CreateReferenceSource(IReadOnlyList<DisasterCard> cards, string version)
        {
            return new InMemoryDisasterCardReferenceSource(cards.ToImmutableArray(), version);
        }

        private static InMemoryDisasterCardReferenceSource CreateReferenceSource(IReadOnlyList<DisasterCard> cards)
        {
            return CreateReferenceSource(cards, _version);
        }
    }
}
