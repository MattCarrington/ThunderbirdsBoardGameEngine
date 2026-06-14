using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.AccessibleLocations.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Mappers;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.Movement.Services
{
    public class MovementClientServiceTests
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
                Messages = ["Movement is valid."]
            };

            var apiResult = ApiResult<ValidateMovementResponseDto>.SuccessResult(expectedResponse, HttpStatusCode.OK);

            var movementClient = Substitute.For<IMovementClient>();
            movementClient
                .ValidateMovementAsync(
                    Arg.Is<string>(x => x == ThunderbirdCode),
                    Arg.Is<ValidateMovementRequestDto>(dto => dto.StartLocation == StartLocationCode && dto.DestinationLocation == DestinationLocationCode),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(apiResult));

            var service = CreateService(movementClient);

            // Act
            var result = await service.ValidateMovementAsync(ThunderbirdCode, StartLocationCode, DestinationLocationCode);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsValid);
            Assert.NotEmpty(result.Route);
            Assert.Equal(expectedResponse.ActionPointCost, result.ActionPointCost);
            Assert.Equal(expectedResponse.SpacesTravelled, result.SpacesTravelled);
            Assert.Equal(expectedResponse.TopSpeed, result.TopSpeed);
            Assert.Equal(expectedResponse.Messages, result.Messages);
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

            var service = CreateService(movementClient);

            // Act
            var result = await service.ValidateMovementAsync(ThunderbirdCode, StartLocationCode, DestinationLocationCode);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAccessibleLocationsAsync_WhenResponseIsSuccess_ReturnsAccessibleLocations()
        {
            // Arrange        
            var expectedResponse = new AccessibleLocationsResponseDto
            {
                AccessibleLocations = ["LOC001", "LOC002", "LOC003"]
            };

            var apiResult = ApiResult<AccessibleLocationsResponseDto>.SuccessResult(expectedResponse, HttpStatusCode.OK);

            var movementClient = Substitute.For<IMovementClient>();
            movementClient
                .GetAccessibleLocationsAsync(
                    Arg.Is<string>(x => x == ThunderbirdCode),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(apiResult));

            var service = CreateService(movementClient);

            // Act
            var result = await service.GetAccessibleLocationsAsync(ThunderbirdCode);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(expectedResponse.AccessibleLocations.Count, result.Count);
        }

        [Fact]
        public async Task GetAccessibleLocationsAsync_WhenResponseIsError_ReturnsEmptyList()
        {
            // Arrange        
            var apiResult = ApiResult<AccessibleLocationsResponseDto>.Failure("Error", HttpStatusCode.BadRequest);

            var movementClient = Substitute.For<IMovementClient>();
            movementClient
                .GetAccessibleLocationsAsync(
                    Arg.Is<string>(x => x == ThunderbirdCode),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(apiResult));

            var service = CreateService(movementClient);

            // Act
            var result = await service.GetAccessibleLocationsAsync(ThunderbirdCode);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        private static MovementClientService CreateService(IMovementClient movementClient)
        {
            var catalog = Substitute.For<ILocationDefinitionCatalog>();
            catalog.TryGetByCode(Arg.Any<LocationCode>(), out Arg.Any<ReferenceLocationDefinition>()).Returns(x =>
            {
                var code = (LocationCode)x[0];
                var location = new ReferenceLocationDefinition(code, $"Location {code.Value}", MovementDomain.Earth);
                x[1] = location;
                return true;
            });

            var resultMapper = new MovementResultMapper(catalog);
            var locationsMapper = new MovementLocationOptionsMapper(catalog);

            return new MovementClientService(movementClient, resultMapper, locationsMapper);
        }
    }
}
