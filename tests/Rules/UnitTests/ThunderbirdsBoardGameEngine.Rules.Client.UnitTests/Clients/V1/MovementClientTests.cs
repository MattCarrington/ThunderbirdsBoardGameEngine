using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.Rules.Client.Clients.V1;
using ThunderbirdsBoardGameEngine.Rules.Client.UnitTests.Helpers;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.AccessibleLocations.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Stubs;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;

namespace ThunderbirdsBoardGameEngine.Rules.Client.UnitTests.Clients.V1
{
    public class MovementClientTests
    {
        private const string ThunderbirdCode = "thunderbird-001";

        [Fact]
        public async Task ValidateMovementAsync_WhenCalled_ShouldCallCorrectEndpoint()
        {
            // Arrange
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var apiResult = CreateValidateMovementSuccessApiResult();

            var client = CreateMovementClient(stubHandler, apiResult);

            var request = CreateRequest();

            // Act
            _ = await client.ValidateMovementAsync(ThunderbirdCode, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal($"http://localhost/api/rules/movement/{ThunderbirdCode}/validate", stubHandler.CapturedRequest?.RequestUri?.ToString());
        }

        [Fact]
        public async Task ValidateMovementAsync_WhenCalled_ShouldCallResponseHandler()
        {
            // Arrange
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var httpClient = new HttpClient(stubHandler)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var apiResult = CreateValidateMovementSuccessApiResult();

            var handler = HttpResponseHandlerHelpers.CreateMockHttpResponseHandler(apiResult);

            var client = new MovementClient(httpClient, handler);

            var request = CreateRequest();

            // Act
            _ = await client.ValidateMovementAsync(ThunderbirdCode, request, TestContext.Current.CancellationToken);

            // Assert
            await handler.Received(1).HandleResponseAsync<ValidateMovementResponseDto>(Arg.Any<HttpResponseMessage>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ValidateMovementAsync_WhenResponseIsSuccessful_ShouldReturnSuccessApiResult()
        {
            // Arrange
            var apiResult = CreateValidateMovementSuccessApiResult();

            if (apiResult.Data == null)
            {
                throw new InvalidOperationException("API result data should not be null in this test.");
            }

            var client = CreateMovementClient(apiResult);

            var request = CreateRequest();

            // Act
            var result = await client.ValidateMovementAsync(ThunderbirdCode, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.Data);
            Assert.Equal(apiResult.Data.ActionPointCost, result.Data.ActionPointCost);
            Assert.Equal(apiResult.Data.IsValid, result.Data.IsValid);
            Assert.Equal(apiResult.Data.SpacesTravelled, result.Data.SpacesTravelled);
            Assert.Equal(apiResult.Data.TopSpeed, result.Data.TopSpeed);
            Assert.NotEmpty(result.Data.Route);
            Assert.NotEmpty(result.Data.Messages);
            Assert.Equal(apiResult.Data.Route, result.Data.Route);
            Assert.Equal(apiResult.Data.Messages, result.Data.Messages);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, "Thunderbird Not Found")]
        [InlineData(HttpStatusCode.InternalServerError, "Internal Server Error")]
        public async Task ValidateMovementAsync_WhenApiReturnsError_ReturnsFailure(HttpStatusCode statusCode, string errorMessage)
        {
            // Arrange
            var apiResult = ApiResult<ValidateMovementResponseDto>.Failure(errorMessage, statusCode);

            var client = CreateMovementClient(apiResult);

            var request = CreateRequest();

            // Act
            var result = await client.ValidateMovementAsync(ThunderbirdCode, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(statusCode, result.StatusCode);
            Assert.Equal(errorMessage, result.ErrorMessage);
            Assert.Null(result.Data);
        }

        [Theory]
        [ClassData(typeof(WhiteSpaceStringData))]
        public async Task ValidateMovementAsync_WhenDisasterCardCodeWhiteSpace_ThrowsArgumentException(string input)
        {
            // Arrange
            var apiResult = CreateValidateMovementSuccessApiResult();

            var client = CreateMovementClient(apiResult);

            var request = new ValidateMovementRequestDto
            {
                StartLocation = "HQ",
                DestinationLocation = "home"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => client.ValidateMovementAsync(input, request, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ValidateMovementAsync_WhenThunderbirdCodeIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var apiResult = CreateValidateMovementSuccessApiResult();

            var client = CreateMovementClient(apiResult);

            var request = new ValidateMovementRequestDto
            {
                StartLocation = "HQ",
                DestinationLocation = "home"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.ValidateMovementAsync(null!, request, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ValidateMovementAsync_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var apiResult = CreateValidateMovementSuccessApiResult();

            var client = CreateMovementClient(apiResult);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.ValidateMovementAsync(ThunderbirdCode, null!, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task GetAccessibleLocationsAsync_WhenCalled_ShouldCallCorrectEndpoint()
        {
            // Arrange
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var apiResult = CreateAccessibleLocationsSuccessApiResult();

            var client = CreateMovementClient(stubHandler, apiResult);

            // Act
            _ = await client.GetAccessibleLocationsAsync(ThunderbirdCode, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal($"http://localhost/api/rules/movement/{ThunderbirdCode}/accessible-locations", stubHandler.CapturedRequest?.RequestUri?.ToString());
        }

        [Fact]
        public async Task GetAccessibleLocationsAsync_WhenResponseIsSuccessful_ShouldReturnSuccessApiResult()
        {
            // Arrange
            var apiResult = CreateAccessibleLocationsSuccessApiResult();

            if (apiResult.Data == null)
            {
                throw new InvalidOperationException("API result data should not be null in this test.");
            }

            var client = CreateMovementClient(apiResult);

            // Act
            var result = await client.GetAccessibleLocationsAsync(ThunderbirdCode, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data.AccessibleLocations);
            Assert.Equal(apiResult.Data.AccessibleLocations, result.Data.AccessibleLocations);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, "Thunderbird Not Found")]
        [InlineData(HttpStatusCode.InternalServerError, "Internal Server Error")]
        public async Task GetAccessibleLocationsAsync_WhenApiReturnsError_ReturnsFailure(HttpStatusCode statusCode, string errorMessage)
        {
            // Arrange
            var apiResult = ApiResult<AccessibleLocationsResponseDto>.Failure(errorMessage, statusCode);

            var client = CreateMovementClient(apiResult);

            // Act
            var result = await client.GetAccessibleLocationsAsync(ThunderbirdCode, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(statusCode, result.StatusCode);
            Assert.Equal(errorMessage, result.ErrorMessage);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetAccessibleLocationsAsync_WhenThunderbirdCodeIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var apiResult = CreateAccessibleLocationsSuccessApiResult();

            var client = CreateMovementClient(apiResult);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetAccessibleLocationsAsync(null!, TestContext.Current.CancellationToken));
        }

        private static ValidateMovementRequestDto CreateRequest()
        {
            var request = new ValidateMovementRequestDto
            {
                StartLocation = "HQ",
                DestinationLocation = "home"
            };

            return request;
        }

        private static ApiResult<ValidateMovementResponseDto> CreateValidateMovementSuccessApiResult()
        {
            var responseDto = new ValidateMovementResponseDto
            {
                IsValid = true,
                ActionPointCost = 3,
                SpacesTravelled = 2,
                TopSpeed = 120,
                Route = ["HQ", "street", "home"],
                Messages = ["Movement is valid."]
            };

            return ApiResult<ValidateMovementResponseDto>.SuccessResult(responseDto, HttpStatusCode.OK);
        }

        private static ApiResult<AccessibleLocationsResponseDto> CreateAccessibleLocationsSuccessApiResult()
        {
            var responseDto = new AccessibleLocationsResponseDto
            {
                AccessibleLocations = ["HQ", "street", "home"]
            };

            return ApiResult<AccessibleLocationsResponseDto>.SuccessResult(responseDto, HttpStatusCode.OK);
        }

        private static MovementClient CreateMovementClient<T>(ApiResult<T> apiResult)
        {
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            return CreateMovementClient(stubHandler, apiResult);
        }

        private static MovementClient CreateMovementClient<T>(HttpMessageHandler httpMessageHandler, ApiResult<T> apiResult)
        {
            var httpClient = new HttpClient(httpMessageHandler)
            {
                BaseAddress = new Uri("http://localhost")
            };

            var handler = HttpResponseHandlerHelpers.CreateMockHttpResponseHandler(apiResult);

            return new MovementClient(httpClient, handler);
        }
    }
}
