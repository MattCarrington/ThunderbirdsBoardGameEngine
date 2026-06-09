using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.UI.Features.Movement;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.Services
{
    public class ValidateMovementServiceTests
    {
        private static string ThunderbirdCode => "TB001";

        private static string StartLocationCode => "LOC001";

        private static string DestinationLocationCode => "LOC002";

        [Fact]
        public async Task ValidateMovementAsync_WhenResponseIsSuccess_ReturnsValidateMovementResponseDto()
        {
            // Arrange        
            var expectedResponse = new ValidateMovementResponseDto
            {
                IsValid = true,
                Route = ["LOC001", "LOC002"],
                ActionPointCost = 2,
                SpacesTravelled = 1,
                TopSpeed = 3,
                Message = ["Movement is valid."]
            };

            var apiResult = ApiResult<ValidateMovementResponseDto>.SuccessResult(expectedResponse, HttpStatusCode.OK);

            var movementClient = Substitute.For<IMovementClient>();
            movementClient
                .ValidateMovementAsync(
                    Arg.Is<string>(x => x == ThunderbirdCode),
                    Arg.Is<ValidateMovementRequestDto>(dto => dto.StartLocation == StartLocationCode && dto.DestinationLocation == DestinationLocationCode),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(apiResult));

            var service = new ValidateMovementService(movementClient);

            // Act
            var result = await service.ValidateMovementAsync(ThunderbirdCode, StartLocationCode, DestinationLocationCode);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsValid);
            Assert.Equal(expectedResponse.Route, result.Route);
            Assert.Equal(expectedResponse.ActionPointCost, result.ActionPointCost);
            Assert.Equal(expectedResponse.SpacesTravelled, result.SpacesTravelled);
            Assert.Equal(expectedResponse.TopSpeed, result.TopSpeed);
            Assert.Equal(expectedResponse.Message, result.Message);
        }

        [Fact]
        public async Task ValidateMovementAsync_WhenResponseIsError_ReturnsNull()
        {
            // Arrange        
            var apiResult = ApiResult<ValidateMovementResponseDto>.Failure("Error", HttpStatusCode.BadRequest);

            var movementClient = Substitute.For<IMovementClient>();
            movementClient
                .ValidateMovementAsync(
                    Arg.Is<string>(x => x == ThunderbirdCode),
                    Arg.Is<ValidateMovementRequestDto>(dto => dto.StartLocation == StartLocationCode && dto.DestinationLocation == DestinationLocationCode),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(apiResult));

            var service = new ValidateMovementService(movementClient);

            // Act
            var result = await service.ValidateMovementAsync(ThunderbirdCode, StartLocationCode, DestinationLocationCode);

            // Assert
            Assert.Null(result);
        }
    }
}
