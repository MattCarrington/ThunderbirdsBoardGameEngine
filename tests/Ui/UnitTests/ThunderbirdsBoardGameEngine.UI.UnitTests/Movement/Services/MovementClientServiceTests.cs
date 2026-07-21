using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
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
            var result = await service.ValidateMovementAsync(ThunderbirdCode, StartLocationCode, DestinationLocationCode, Array.Empty<string>());

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
            var result = await service.ValidateMovementAsync(ThunderbirdCode, StartLocationCode, DestinationLocationCode, Array.Empty<string>());

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

        [Fact]
        public async Task GetAccessibleLocationsAsync_WhenCalledMultipleTimesForSameThunderbird_CallsClientOnce()
        {
            // Arrange        
            var apiResult = CreateValidAccessibleLocationsResponseDto();

            var movementClient = Substitute.For<IMovementClient>();
            movementClient
                .GetAccessibleLocationsAsync(
                    Arg.Is<string>(x => x == ThunderbirdCode),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(apiResult));

            var service = CreateService(movementClient);

            // Act
            _ = await service.GetAccessibleLocationsAsync(ThunderbirdCode);
            _ = await service.GetAccessibleLocationsAsync(ThunderbirdCode);

            // Assert
            await AssertClientCalled(movementClient, ThunderbirdCode, 1);
        }

        [Fact]
        public async Task GetAccessibleLocationsAsync_WhenCalledMultipleTimesForDifferentThunderbirds_CallsClientForEachThunderbird()
        {
            // Arrange        
            var apiResult = CreateValidAccessibleLocationsResponseDto();

            var movementClient = Substitute.For<IMovementClient>();
            movementClient
                .GetAccessibleLocationsAsync(
                    Arg.Any<string>(),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(apiResult));

            var service = CreateService(movementClient);

            // Act
            _ = await service.GetAccessibleLocationsAsync("TB001");
            _ = await service.GetAccessibleLocationsAsync("TB002");

            // Assert
            await AssertClientCalled(movementClient, "TB001", 1);
            await AssertClientCalled(movementClient, "TB002", 1);
        }

        [Fact]
        public async Task GetAccessibleLocationsAsync_WhenFirstRequestIsFailure_DoesNotCache()
        {
            // Arrange
            var failureResult = ApiResult<AccessibleLocationsResponseDto>.Failure("Error", HttpStatusCode.BadRequest);

            var successResult = CreateValidAccessibleLocationsResponseDto();

            var movementClient = Substitute.For<IMovementClient>();
            movementClient
                .GetAccessibleLocationsAsync(
                    Arg.Is<string>(x => x == ThunderbirdCode),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(failureResult), Task.FromResult(successResult));

            var service = CreateService(movementClient);

            // Act
            var result1 = await service.GetAccessibleLocationsAsync(ThunderbirdCode);
            var result2 = await service.GetAccessibleLocationsAsync(ThunderbirdCode);

            // Assert
            await AssertClientCalled(movementClient, ThunderbirdCode, 2);

            Assert.Empty(result1);
            Assert.NotEmpty(result2);
        }

        [Fact]
        public async Task GetAccessibleLocationsAsync_WhenFirstRequestIsNull_DoesNotCache()
        {
            // Arrange
            var nullResult = ApiResult<AccessibleLocationsResponseDto>.SuccessResult(null, HttpStatusCode.OK);

            var successResult = CreateValidAccessibleLocationsResponseDto();

            var movementClient = Substitute.For<IMovementClient>();
            movementClient
                .GetAccessibleLocationsAsync(
                    Arg.Is<string>(x => x == ThunderbirdCode),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(nullResult), Task.FromResult(successResult));

            var service = CreateService(movementClient);

            // Act
            var result1 = await service.GetAccessibleLocationsAsync(ThunderbirdCode);
            var result2 = await service.GetAccessibleLocationsAsync(ThunderbirdCode);

            // Assert
            await AssertClientCalled(movementClient, ThunderbirdCode, 2);

            Assert.Empty(result1);
            Assert.NotEmpty(result2);
        }

        [Fact]
        public async Task GetAccessibleLocationsAsync_WhenFirstRequestIsEmpty_DoesNotCallClientAgain()
        {
            // Arrange
            var emptyResult = ApiResult<AccessibleLocationsResponseDto>.SuccessResult(new AccessibleLocationsResponseDto { AccessibleLocations = Array.Empty<string>() }, HttpStatusCode.OK);

            var successResult = CreateValidAccessibleLocationsResponseDto();

            var movementClient = Substitute.For<IMovementClient>();
            movementClient
                .GetAccessibleLocationsAsync(
                    Arg.Is<string>(x => x == ThunderbirdCode),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(emptyResult), Task.FromResult(successResult));

            var service = CreateService(movementClient);

            // Act
            var result1 = await service.GetAccessibleLocationsAsync(ThunderbirdCode);
            var result2 = await service.GetAccessibleLocationsAsync(ThunderbirdCode);

            // Assert
            await AssertClientCalled(movementClient, ThunderbirdCode, 1);

            Assert.Empty(result1);
            Assert.Empty(result2);
        }

        private static ApiResult<AccessibleLocationsResponseDto> CreateValidAccessibleLocationsResponseDto()
        {
            var responseDto = new AccessibleLocationsResponseDto
            {
                AccessibleLocations = ["LOC001", "LOC002", "LOC003"]
            };

            return ApiResult<AccessibleLocationsResponseDto>.SuccessResult(responseDto, HttpStatusCode.OK);
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

        private static Task<ApiResult<AccessibleLocationsResponseDto>> AssertClientCalled(IMovementClient client, string thunderbirdCode, int expectedCallCount)
        {
            return client.Received(expectedCallCount).GetAccessibleLocationsAsync(Arg.Is<string>(x => x == thunderbirdCode), Arg.Any<CancellationToken>());
        }
    }
}
