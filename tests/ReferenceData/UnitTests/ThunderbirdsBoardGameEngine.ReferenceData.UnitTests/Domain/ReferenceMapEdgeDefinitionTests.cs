using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Domain
{
    public class ReferenceMapEdgeDefinitionTests
    {
        private static LocationCode ValidLocationCode1 => new("location-1");

        private static LocationCode ValidLocationCode2 => new("location-2");

        private static TraversalDomain ValidTraversalDomain => TraversalDomain.Earth;

        [Fact]
        public void Constructor_WhenAllInputsValid_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferenceMapEdgeDefinition(
                edge1: ValidLocationCode1,
                edge2: ValidLocationCode2,
                edgeType: ValidTraversalDomain
            );

            // Assert
            Assert.Equal(ValidLocationCode1, result.Edge1);
            Assert.Equal(ValidLocationCode2, result.Edge2);
            Assert.Equal(ValidTraversalDomain, result.EdgeType);
        }

        [Fact]
        public void Constructor_WhenEdge1EqualsEdge2_ThrowsArgumentException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ReferenceMapEdgeDefinition(
                edge1: ValidLocationCode1,
                edge2: ValidLocationCode1,
                edgeType: ValidTraversalDomain
            ));
        }
    }
}
