using System.Collections.ObjectModel;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Movement
{
    public class MovementValidatorTests
    {
        private readonly LocationContribution _locationA = new(new("A"), MovementDomain.Earth);
        private readonly LocationContribution _locationB = new(new("B"), MovementDomain.Earth);

        private readonly ThunderbirdContribution _thunderbird = new(new("Thunderbird 1"), MovementDomain.Earth, 1);

        private readonly LocationCode _invalidLocation = new("Invalid");

        [Fact]
        public void Validate_ValidRequest_ReturnsValidResult()
        {
            // Arrange
            var topography = CreateTopography();

            var request = new MovementInput(_thunderbird, topography, _locationA.Key, _locationB.Key);

            var validator = new MovementValidator();

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsValid);
            Assert.Null(result.ErrorCode);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void Validate_InvalidStartLocation_ReturnsInvalidResult()
        {
            // Arrange
            var topography = CreateTopography();

            var request = new MovementInput(_thunderbird, topography, _invalidLocation, _locationB.Key);

            var validator = new MovementValidator();

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessage);
            Assert.Equal("Unknown start location", result.ErrorMessage);
            Assert.NotNull(result.ErrorCode);
            Assert.Equal(_invalidLocation.Value, result.ErrorCode);
        }

        [Fact]
        public void Validate_InvalidEndLocation_ReturnsInvalidResult()
        {
            // Arrange
            var topography = CreateTopography();

            var request = new MovementInput(_thunderbird, topography, _locationA.Key, _invalidLocation);

            var validator = new MovementValidator();

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessage);
            Assert.Equal("Unknown destination location", result.ErrorMessage);
            Assert.NotNull(result.ErrorCode);
            Assert.Equal(_invalidLocation.Value, result.ErrorCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Validate_InvalidMovementRange_ReturnsInvalidResult(int topSpeed)
        {
            // Arrange
            var topography = CreateTopography();

            var invalidThunderbird = new ThunderbirdContribution(new("Invalid Thunderbird"), MovementDomain.Earth, topSpeed);

            var request = new MovementInput(invalidThunderbird, topography, _locationA.Key, _locationB.Key);

            var validator = new MovementValidator();

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessage);
            Assert.Equal($"Invalid movement range: {topSpeed}", result.ErrorMessage);
            Assert.NotNull(result.ErrorCode);
            Assert.Equal(invalidThunderbird.Key.Value, result.ErrorCode);
        }

        private Topography CreateTopography()
        {
            var locations = new Collection<LocationContribution>
            {
                _locationA,
                _locationB
            };

            return new Topography(locations, Array.Empty<ReferenceMapEdgeDefinition>());
        }
    }
}
