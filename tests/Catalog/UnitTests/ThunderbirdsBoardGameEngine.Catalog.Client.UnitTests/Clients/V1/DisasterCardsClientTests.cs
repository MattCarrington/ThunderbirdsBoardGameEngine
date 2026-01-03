using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Client.Clients.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Routing.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.Handlers;
using ThunderbirdsBoardGameEngine.TestUtils.Stubs;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.UnitTests.Clients.V1
{
    public class DisasterCardsClientTests
    {
        [Fact]
        public async Task GetAllAsync_WhenCalled_ShouldCallCorrectEndpoint()
        {
            // Arrange
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var httpClient = new HttpClient(stubHandler)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var apiResult = ApiResult<IReadOnlyList<DisasterCardDto>>.SuccessResult([], HttpStatusCode.OK);

            var handler = CreateMockHttpResponseHandler(apiResult);

            var client = new DisasterCardsClient(httpClient, handler);

            // Act
            _ = await client.GetAllAsync();

            // Assert
            Assert.Equal($"http://localhost/{ApiRoutes.DisasterCards}", stubHandler.CapturedRequest?.RequestUri?.ToString());
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ShouldCallResponseHandler()
        {
            // Arrange
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var httpClient = new HttpClient(stubHandler)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var apiResult = ApiResult<IReadOnlyList<DisasterCardDto>>.SuccessResult([], HttpStatusCode.OK);

            var handler = CreateMockHttpResponseHandler(apiResult);

            var client = new DisasterCardsClient(httpClient, handler);

            // Act
            _ = await client.GetAllAsync();

            // Assert
            await handler.Received(1).HandleResponseAsync<IReadOnlyList<DisasterCardDto>>(Arg.Any<HttpResponseMessage>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllAsync_WhenApiReturnsSuccess_ReturnsCards()
        {
            // Arrange
            var disasterCardDtos = new List<DisasterCardDto>
            {
                new()
                {
                    Id = 1,
                    Name = "Card A",
                    Location = "Earth"

                },
                new()
                {
                    Id = 2,
                    Name = "Card B",
                    Location = "Space"
                }
            };

            var apiResult = ApiResult<IReadOnlyList<DisasterCardDto>>.SuccessResult(disasterCardDtos, HttpStatusCode.OK);

            var client = CreateDisasterCardClient(apiResult);

            // Act
            var result = await client.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.Data);
            Assert.Equal(disasterCardDtos.Count, result.Data.Count);

            DisasterCardDtoAssertions.AssertOrderSensitive(disasterCardDtos, result.Data.ToList());
        }

        [Fact]
        public async Task GetAllAsync_WhenApiReturnsEmptyList_ReturnsEmptyDataList()
        {
            // Arrange
            var apiResult = ApiResult<IReadOnlyList<DisasterCardDto>>.SuccessResult([], HttpStatusCode.OK);           

            var client = CreateDisasterCardClient(apiResult);

            // Act
            var result = await client.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, "Disaster Cards Not Found")]
        [InlineData(HttpStatusCode.InternalServerError, "Internal Server Error")]
        public async Task GetAllAsync_WhenApiReturnsError_ReturnsFailure(HttpStatusCode statusCode, string errorMessage)
        {
            // Arrange
            var apiResult = ApiResult<IReadOnlyList<DisasterCardDto>>.Failure(errorMessage, statusCode);

            var client = CreateDisasterCardClient(apiResult);

            // Act
            var result = await client.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(statusCode, result.StatusCode);
            Assert.Equal(errorMessage, result.ErrorMessage);
            Assert.Null(result.Data);
        }

        private static IHttpResponseHandler CreateMockHttpResponseHandler(ApiResult<IReadOnlyList<DisasterCardDto>> apiResult)
        {
            var handler = Substitute.For<IHttpResponseHandler>();
            handler.HandleResponseAsync<IReadOnlyList<DisasterCardDto>>(Arg.Any<HttpResponseMessage>(), Arg.Any<CancellationToken>())
                .Returns(apiResult);

            return handler;
        }

        private static DisasterCardsClient CreateDisasterCardClient(ApiResult<IReadOnlyList<DisasterCardDto>> apiResult)
        {
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var httpClient = new HttpClient(stubHandler)
            {
                BaseAddress = new Uri("http://example.test")
            };

            var handler = CreateMockHttpResponseHandler(apiResult);

            return new DisasterCardsClient(httpClient, handler);
        }
    }
}
