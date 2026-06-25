using ThunderbirdsBoardGameEngine.Api.Exceptions;
using ThunderbirdsBoardGameEngine.Api.Mappers.Rules.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Mappers.Rules.V1
{
    public class MovementMappingExtensionsTests
    {
        [Fact]
        public void ToQuery_ValidDto_ReturnsExpectedQuery()
        {
            // Arrange
            var dto = new ValidateMovementRequestDto
            {
                StartLocation = "location1",
                DestinationLocation = "location2"
            };

            // Act
            var result = dto.ToQuery("thunderbird4");

            // Assert
            Assert.Equal(new ThunderbirdCode("thunderbird4"), result.Thunderbird);
            Assert.Equal(new LocationCode("location1"), result.Start);
            Assert.Equal(new LocationCode("location2"), result.Destination);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void ToQuery_StartLocationNullOrEmpty_ThrowsBadRequestException(string? startLocation)
        {
            // Arrange
            var dto = new ValidateMovementRequestDto
            {
                StartLocation = startLocation,
                DestinationLocation = "location2"
            };

            // Act & Assert
            var ex = Assert.Throws<BadRequestException>(() => dto.ToQuery("thunderbird4"));
            Assert.Equal("Start location must be provided.", ex.Message);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void ToQuery_DestinationLocationNullOrEmpty_ThrowsBadRequestException(string? destinationLocation)
        {
            // Arrange
            var dto = new ValidateMovementRequestDto
            {
                StartLocation = "location1",
                DestinationLocation = destinationLocation
            };

            // Act & Assert
            var ex = Assert.Throws<BadRequestException>(() => dto.ToQuery("thunderbird4"));
            Assert.Equal("Destination location must be provided.", ex.Message);
        }

        [Fact]
        public void ToDto_ValidResponse_ReturnsExpectedDto()
        {
            // Arrange
            var response = new ValidateMovementResponse
            (
                IsValid: true,
                ActionPointCost: 3,
                SpacesTravelled: 2,
                Messages: ["Move successful."],
                Route: [new LocationCode("location1"), new LocationCode("location2")],
                TopSpeed: 5
            );

            // Act
            var result = response.ToDto();

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(3, result.ActionPointCost);
            Assert.Equal(2, result.SpacesTravelled);
            Assert.Equal(2, result.Route.Count);
            Assert.Equal(5, result.TopSpeed);

            var message = Assert.Single(result.Messages);
            Assert.Equal("Move successful.", message);

            Assert.NotEmpty(result.Route);
            Assert.Contains("location1", result.Route);
            Assert.Contains("location2", result.Route);
        }
    }
}
