using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.UnitTests.Catalogs
{
    public class EventCardDefinitionCatalogTests
    {
        [Fact]
        public void Exists_WhenExistingCardCode_ReturnsTrue()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.Exists(new CardCode("event-1"));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Exists_WhenNonExistingCardCode_ReturnsFalse()
        {
            // Arrange
            var catalog = CreateCatalog();

            // Act
            var result = catalog.Exists(new CardCode("non-existing-code"));

            // Assert
            Assert.False(result);
        }

        private static EventCardDefinitionCatalog CreateCatalog()
        {
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithEventCard("event-1", "Event 1")
                .WithEventCard("event-2", "Event 2")
                .Build();

            return new EventCardDefinitionCatalog(snapshot);
        }
    }
}
