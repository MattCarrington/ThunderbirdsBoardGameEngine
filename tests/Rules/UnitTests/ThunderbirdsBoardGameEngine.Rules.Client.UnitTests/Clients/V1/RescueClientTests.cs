using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.Handlers;
using ThunderbirdsBoardGameEngine.Rules.Client.Clients.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Stubs;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;

namespace ThunderbirdsBoardGameEngine.Rules.Client.UnitTests.Clients.V1
{
    public class RescueClientTests
    {
        [Fact]
        public async Task CalculateRescueTargetAsync_WhenCalled_ShouldCallCorrectEndpoint()
        {
            // Arrange
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var apiResult = CreateSuccessApiResult();

            var client = CreateRescueClient(stubHandler, apiResult);

            var (cardCode, request) = CreateRequest();

            // Act
            _ = await client.CalculateRescueTargetAsync(cardCode, request);

            // Assert
            Assert.Equal($"http://localhost/api/rules/rescue/{cardCode}/target", stubHandler.CapturedRequest?.RequestUri?.ToString());
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenCalled_ShouldCallResponseHandler()
        {
            // Arrange
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var httpClient = new HttpClient(stubHandler)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var apiResult = CreateSuccessApiResult();

            var handler = CreateMockHttpResponseHandler(apiResult);

            var client = new RescueClient(httpClient, handler);

            var (cardCode, request) = CreateRequest();

            // Act
            _ = await client.CalculateRescueTargetAsync(cardCode, request);

            // Assert
            await handler.Received(1).HandleResponseAsync<CalculateRescueTargetResponseDto>(Arg.Any<HttpResponseMessage>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenResponseIsSuccessful_ShouldReturnSuccessApiResult()
        {
            // Arrange
            var apiResult = CreateSuccessApiResult();

            if (apiResult.Data == null)
            {
                throw new InvalidOperationException("API result data should not be null in this test.");
            }

            var client = CreateRescueClient(apiResult);

            var (cardCode, request) = CreateRequest();

            // Act
            var result = await client.CalculateRescueTargetAsync(cardCode, request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.Data);
            Assert.Equal(apiResult.Data.TargetNumber, result.Data.TargetNumber);
            Assert.Equal(apiResult.Data.TotalBonus, result.Data.TotalBonus);
            Assert.Empty(result.Data.AppliedDisasterBonuses);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, "Disaster Cards Not Found")]
        [InlineData(HttpStatusCode.InternalServerError, "Internal Server Error")]
        public async Task CalculateRescueTargetAsync_WhenApiReturnsError_ReturnsFailure(HttpStatusCode statusCode, string errorMessage)
        {
            // Arrange
            var apiResult = ApiResult<CalculateRescueTargetResponseDto>.Failure(errorMessage, statusCode);

            var client = CreateRescueClient(apiResult);

            var (cardCode, request) = CreateRequest();

            // Act
            var result = await client.CalculateRescueTargetAsync(cardCode, request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(statusCode, result.StatusCode);
            Assert.Equal(errorMessage, result.ErrorMessage);
            Assert.Null(result.Data);
        }

        [Theory]
        [InlineData("they call him", "they%20call%20him")]
        [InlineData("they/call/him", "they%2Fcall%2Fhim")]
        [InlineData("they?call#him", "they%3Fcall%23him")]
        public async Task CalculateRescueTargetAsync_WhenCardCodeContainsUnsafeCharacters_EncodesCardCode(string rawCardCode, string expectedEncodedCode)
        {
            // Arrange
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            var apiResult = CreateSuccessApiResult();

            var client = CreateRescueClient(stubHandler, apiResult);

            var request = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = []
            };

            // Act
            await client.CalculateRescueTargetAsync(rawCardCode, request);

            // Assert
            Assert.Equal(
                $"/api/rules/rescue/{expectedEncodedCode}/target",
                stubHandler.CapturedRequest?.RequestUri?.AbsolutePath);
        }

        [Theory]
        [ClassData(typeof(WhitespaceStringData))]
        public async Task CalculateRescueTargetAsync_WhenDisasterCardCodeWhiteSpace_ThrowsArgumentException(string input)
        {
            // Arrange
            var apiResult = CreateSuccessApiResult();

            var client = CreateRescueClient(apiResult);

            var request = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = []
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => client.CalculateRescueTargetAsync(input, request));
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenDisasterCardCodeIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var apiResult = CreateSuccessApiResult();

            var client = CreateRescueClient(apiResult);

            var request = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = []
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.CalculateRescueTargetAsync(null!, request));
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var apiResult = CreateSuccessApiResult();

            var client = CreateRescueClient(apiResult);

            var cardCode = "DC001";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.CalculateRescueTargetAsync(cardCode, null!));
        }

        private static (string, CalculateRescueTargetRequestDto) CreateRequest()
        {
            var request = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = Array.Empty<string>()
            };

            var cardCode = "DC001";

            return (cardCode, request);
        }

        private static ApiResult<CalculateRescueTargetResponseDto> CreateSuccessApiResult()
        {
            var responseDto = new CalculateRescueTargetResponseDto
            {
                AppliedDisasterBonuses = Array.Empty<AppliedDisasterBonusDto>(),
                TargetNumber = 5,
                TotalBonus = 0
            };

            return ApiResult<CalculateRescueTargetResponseDto>.SuccessResult(responseDto, HttpStatusCode.OK);
        }

        private static IHttpResponseHandler CreateMockHttpResponseHandler(ApiResult<CalculateRescueTargetResponseDto> apiResult)
        {
            var handler = Substitute.For<IHttpResponseHandler>();
            handler.HandleResponseAsync<CalculateRescueTargetResponseDto>(Arg.Any<HttpResponseMessage>(), Arg.Any<CancellationToken>())
                .Returns(apiResult);

            return handler;
        }

        private static RescueClient CreateRescueClient(ApiResult<CalculateRescueTargetResponseDto> apiResult)
        {
            var stubHandler = new StubHttpMessageHandler("{}", HttpStatusCode.OK);

            return CreateRescueClient(stubHandler, apiResult);
        }

        private static RescueClient CreateRescueClient(HttpMessageHandler httpMessageHandler, ApiResult<CalculateRescueTargetResponseDto> apiResult)
        {
            var httpClient = new HttpClient(httpMessageHandler)
            {
                BaseAddress = new Uri("http://localhost")
            };

            var handler = CreateMockHttpResponseHandler(apiResult);

            return new RescueClient(httpClient, handler);
        }
    }
}
