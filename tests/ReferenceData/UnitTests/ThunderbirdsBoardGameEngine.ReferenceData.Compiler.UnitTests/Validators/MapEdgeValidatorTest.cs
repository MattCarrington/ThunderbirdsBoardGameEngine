using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.UnitTests.Validators
{
    public class MapEdgeValidatorTest
    {
        [Fact]
        public void Validate_WhenAllEdgesValid_DoesNotThrow()
        {
            // Arrange
            var validator = new MapEdgeValidator();

            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithMapEdge("location-1", "location-2")
                .WithMapEdge("location-2", "location-3")
                .WithMapEdge("location-3", "location-1")
                .Build();

            // Act & Assert
            validator.Validate(snapshot);
        }

        [Fact]
        public void Validate_WithInvalidEdge_ThrowsException()
        {
            // Arrange
            var validator = new MapEdgeValidator();

            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithMapEdge("location-1", "location-2")
                .WithMapEdge("location-1", "location-2")
                .Build();

            // Act & Assert
            Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
        }

        [Fact]
        public void Validate_WithInvalidEdgeReversed_ThrowsException()
        {
            var validator = new MapEdgeValidator();

            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithMapEdge("location-1", "location-2")
                .WithMapEdge("location-2", "location-1")
                .Build();

            // Act & Assert
            Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
        }

        [Fact]
        public void Validate_WithInvalidEdgeWithDifferentDomains_ThrowsException()
        {
            // Arrange
            var validator = new MapEdgeValidator();

            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithMapEdge("location-1", "location-2", MovementDomain.Earth)
                .WithMapEdge("location-1", "location-2", MovementDomain.Space)
                .Build();

            // Act & Assert
            Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
        }
    }
}