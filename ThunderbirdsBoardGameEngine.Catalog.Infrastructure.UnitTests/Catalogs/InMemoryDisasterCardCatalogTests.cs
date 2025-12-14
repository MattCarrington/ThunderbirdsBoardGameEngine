using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Catalogs;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Catalogs
{
    public class InMemoryDisasterCardCatalogTests
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
            var catalog = CreateCatalog(_cards, _version);

            // Assert
            Assert.Equal(_version, catalog.Version);
            Assert.Equal(2, catalog.Cards.Length);
            Assert.Contains(catalog.Cards, c => c.Id == 1 && c.Name == "Test 1");
            Assert.Contains(catalog.Cards, c => c.Id == 2 && c.Name == "Test 2");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidVersion_ThrowsArgumentException(string invalidVersion)
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new InMemoryDisasterCardCatalog(_cards, invalidVersion));
        }

        [Fact]
        public void Constructor_WithNullVersion_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new InMemoryDisasterCardCatalog(_cards, null!));
        }

        [Fact]
        public void Constructor_WithEmptyCards_ThrowsArgumentException()
        {
            // Arrange
            var emptyCards = ImmutableArray<DisasterCard>.Empty;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new InMemoryDisasterCardCatalog(emptyCards, _version));
        }

        [Fact]
        public void All_WhenChanged_IsImmutable()
        {
            // Arrange
            var cards = _cards.ToList();

            var catalog = CreateCatalog(cards);

            // Act
            cards.Add(new DisasterCardBuilder().WithId(3).WithName("Test 3").Build());

            // Assert
            Assert.Equal(2, catalog.Cards.Length);
        }

        private static InMemoryDisasterCardCatalog CreateCatalog(IReadOnlyList<DisasterCard> cards, string version)
        {
            return new InMemoryDisasterCardCatalog(cards.ToImmutableArray(), version);
        }

        private static InMemoryDisasterCardCatalog CreateCatalog(IReadOnlyList<DisasterCard> cards)
        {
            return CreateCatalog(cards, _version);
        }
    }
}
