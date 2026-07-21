using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Movement
{
    public class TopographyTests
    {
        [Theory]
        [InlineData(MovementDomain.Earth, "B")]
        [InlineData(MovementDomain.Space, "D")]
        public void GetNeighbours_WhenCalled_ReturnsExpectedNeighbour(MovementDomain domain, string expectedNeighbour)
        {
            // Arrange
            var topography = CreateTopography();

            // Act
            var result = topography.GetNeighbours(new LocationCode("A"), domain).ToList();

            // Assert
            var location = Assert.Single(result);
            Assert.Equal(new LocationCode(expectedNeighbour), location);
        }

        [Fact]
        public void GetNeighbours_WhenMultipleNeighbours_ReturnsAllNeighbours()
        {
            // Arrange
            var topography = CreateTopography();

            // Act
            var result = topography.GetNeighbours(new LocationCode("B"), MovementDomain.Earth).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(new LocationCode("A"), result);
            Assert.Contains(new LocationCode("C"), result);
        }

        [Fact]
        public void GetNeighbours_WhenNoNeighbours_ReturnsEmpty()
        {
            // Arrange
            var topography = CreateTopography();

            // Act
            var result = topography.GetNeighbours(new LocationCode("B"), MovementDomain.Space).ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetNeighbours_WhenLocationNotInTopography_ReturnsEmpty()
        {
            // Arrange
            var topography = CreateTopography();

            // Act
            var result = topography.GetNeighbours(new LocationCode("X"), MovementDomain.Earth).ToList();

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(MovementDomain.Earth, new[] { "A", "B", "C", "D" })]
        [InlineData(MovementDomain.Space, new[] { "A", "D" })]
        public void GetAccessibleLocationsForDomain_WhenCalled_ReturnsExpectedLocations(MovementDomain domain, string[] expectedLocations)
        {
            // Arrange
            var topography = CreateTopography();

            // Act
            var result = topography.GetAccessibleLocationsForDomain(domain);

            // Assert
            Assert.Equal(expectedLocations.Length, result.Count);

            foreach (var location in expectedLocations)
            {
                Assert.Contains(new LocationCode(location), result);
            }
        }

        private static Topography CreateTopography()
        {
            var edges = new List<ReferenceMapEdgeDefinition>
            {
                new(new LocationCode("A"), new LocationCode("B"), MovementDomain.Earth),
                new(new LocationCode("B"), new LocationCode("C"), MovementDomain.Earth),
                new(new LocationCode("C"), new LocationCode("D"), MovementDomain.Earth),
                new(new LocationCode("A"), new LocationCode("D"), MovementDomain.Space)
            };

            return new Topography(edges);
        }
    }
}
