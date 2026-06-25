using NSubstitute;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.UnitTests.Lookups
{
    public class ReferenceLocationLookupTests
    {
        [Fact]
        public void GetAllLocationContributions_WhenCalled_ReturnsMappedContributions()
        {
            // Arrange
            var locationDefinitions = new[]
            {
                new ReferenceLocationDefinition(new LocationCode("location-1"), "Location 1", MovementDomain.Earth),
                new ReferenceLocationDefinition(new LocationCode("location-2"), "Location 2", MovementDomain.Space)
            };


            var catalog = Substitute.For<ILocationDefinitionCatalog>();
            catalog.GetAll().Returns(locationDefinitions.ToImmutableArray());

            var lookup = new ReferenceLocationDefinitionLookup(catalog);

            // Act
            var result = lookup.GetAllLocationContributions();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Key == new LocationCode("location-1") && c.Location == MovementDomain.Earth);
            Assert.Contains(result, c => c.Key == new LocationCode("location-2") && c.Location == MovementDomain.Space);
        }

        [Fact]
        public void Exists_WhenLocationExists_ReturnsTrue()
        {
            // Arrange
            var catalog = Substitute.For<ILocationDefinitionCatalog>();
            catalog.Exists(new LocationCode("location-1")).Returns(true);

            var lookup = new ReferenceLocationDefinitionLookup(catalog);

            // Act
            var result = lookup.Exists(new LocationCode("location-1"));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Exists_WhenLocationDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var catalog = Substitute.For<ILocationDefinitionCatalog>();
            catalog.Exists(new LocationCode("location-1")).Returns(false);

            var lookup = new ReferenceLocationDefinitionLookup(catalog);

            // Act
            var result = lookup.Exists(new LocationCode("location-1"));

            // Assert
            Assert.False(result);
        }
    }
}
