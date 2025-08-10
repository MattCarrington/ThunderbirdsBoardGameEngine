using AutoFixture;
using System.Net;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.GameData.Api.Client.Clients.V1;
using ThunderbirdsBoardGameEngine.GameData.Api.Client.Internal.Routing;
using ThunderbirdsBoardGameEngine.GameData.Api.Client.Internal.Serialization;
using ThunderbirdsBoardGameEngine.GameData.Api.Client.UnitTests.Helpers;
using ThunderbirdsBoardGameEngine.GameData.Api.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Client.UnitTests.Clients.V1
{
    public class DisasterCardClientTests
    {
        private readonly Fixture _fixture = new();
        
        public DisasterCardClientTests()
        {
            _fixture.Customize<DisasterCardDto>(c => c
                .With(d => d.BonusConditions, [.. _fixture.CreateMany<BonusConditionDto>(2)])
                .With(d => d.Rewards, [.. _fixture.CreateMany<RewardDto>(2)]));
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ShouldSetApiVersionHeader()
        {
            // Arrange
            var json = JsonSerializer.Serialize(new List<DisasterCardDto>(), JsonDefaults.CamelCase);

            var stubHandler = new StubHttpMessageHandler(json, HttpStatusCode.OK);

            var httpClient = new HttpClient(stubHandler)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var client = new DisasterCardClient(httpClient);

            // Act
            _ = await client.GetAllAsync();

            // Assert
            Assert.True(httpClient.DefaultRequestHeaders.Contains("X-Api-Version"));
            Assert.Equal("1.0", httpClient.DefaultRequestHeaders.GetValues("X-Api-Version").FirstOrDefault());
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ShouldCallCorrectEndpoint()
        {
            // Arrange
            var json = JsonSerializer.Serialize(new List<DisasterCardDto>(), JsonDefaults.CamelCase);

            var stubHandler = new StubHttpMessageHandler(json, HttpStatusCode.OK);

            var httpClient = new HttpClient(stubHandler)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var client = new DisasterCardClient(httpClient);

            // Act
            _ = await client.GetAllAsync();

            // Assert
            Assert.Equal($"http://localhost/{ApiRoutes.DisasterCard}", stubHandler.CapturedRequest?.RequestUri?.ToString());
        }

        [Fact]
        public async Task GetAllAsync_WhenApiReturnsSuccess_ReturnsCards()
        {
            // Arrange
            var disasterCardDtos = _fixture.CreateMany<DisasterCardDto>(5).ToList();

            var json = JsonSerializer.Serialize(disasterCardDtos, JsonDefaults.CamelCase);

            var client = CreateDisasterCardClient(HttpStatusCode.OK, json);

            // Act
            var result = await client.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.Data);
            Assert.Equal(disasterCardDtos.Count, result.Data.Count);

            for (int i = 0; i < disasterCardDtos.Count; i++)
            {
                DisasterCardDtoAssertions.AssertDisasterCardDtoEqual(disasterCardDtos[i], result.Data[i]);
            }
        }

        [Fact]
        public async Task GetAllAsync_WhenApiReturnsEmptyList_ReturnsEmptyDataList()
        {
            // Arrange
            var json = JsonSerializer.Serialize(new List<DisasterCardDto>(), JsonDefaults.CamelCase);

            var client = CreateDisasterCardClient(HttpStatusCode.OK, json);

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
            var client = CreateDisasterCardClient(statusCode, errorMessage);

            // Act
            var result = await client.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(statusCode, result.StatusCode);
            Assert.Equal(errorMessage, result.ErrorMessage);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetAllAsync_WhenApiReturnsMalformedJson_ReturnsFailure()
        {
            // Arrange
            var client = CreateDisasterCardClient(HttpStatusCode.OK, "Invalid JSON");

            // Act
            var result = await client.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("Deserialization error", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetAllAsync_WhenDeserializedContentIsNull_ReturnsFailure()
        {
            // Arrange
            var client = CreateDisasterCardClient(HttpStatusCode.OK, "null");

            // Act
            var result = await client.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("Deserialized content was null.", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalled_ShouldSetApiVersionHeader()
        {
            var json = JsonSerializer.Serialize(new DisasterCardDto(), JsonDefaults.CamelCase);

            var stubHandler = new StubHttpMessageHandler(json, HttpStatusCode.OK);

            var httpClient = new HttpClient(stubHandler)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var client = new DisasterCardClient(httpClient);

            // Act
            _ = await client.GetByIdAsync(4);

            // Assert
            Assert.True(httpClient.DefaultRequestHeaders.Contains("X-Api-Version"));
            Assert.Equal("1.0", httpClient.DefaultRequestHeaders.GetValues("X-Api-Version").FirstOrDefault());
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalled_ShouldCallCorrectEndpoint()
        {
            // Arrange
            var json = JsonSerializer.Serialize(new DisasterCardDto(), JsonDefaults.CamelCase);

            var stubHandler = new StubHttpMessageHandler(json, HttpStatusCode.OK);

            var httpClient = new HttpClient(stubHandler)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            var client = new DisasterCardClient(httpClient);

            // Act
            _ = await client.GetByIdAsync(4);

            // Assert
            Assert.Equal($"http://localhost/{ApiRoutes.DisasterCard}/4", stubHandler.CapturedRequest?.RequestUri?.ToString());
        }

        [Fact]
        public async Task GetByIdAsync_WhenApiReturnsSuccess_ReturnsCard()
        {
            // Arrange
            var disasterCardDto = _fixture.Create<DisasterCardDto>();

            var json = JsonSerializer.Serialize(disasterCardDto, JsonDefaults.CamelCase);

            var client = CreateDisasterCardClient(HttpStatusCode.OK, json);

            // Act
            var result = await client.GetByIdAsync(disasterCardDto.Id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.Data);

            DisasterCardDtoAssertions.AssertDisasterCardDtoEqual(disasterCardDto, result.Data);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound, "Disaster Card Not Found")]
        [InlineData(HttpStatusCode.InternalServerError, "Internal Server Error")]
        public async Task GetByIdAsync_WhenApiReturnsError_ReturnsFailure(HttpStatusCode statusCode, string errorMessage)
        {
            // Arrange
            var client = CreateDisasterCardClient(statusCode, errorMessage);

            // Act
            var result = await client.GetByIdAsync(999);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(statusCode, result.StatusCode);
            Assert.Equal(errorMessage, result.ErrorMessage);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetByIdAsync_WhenApiReturnsMalformedJson_ReturnsFailure()
        {
            // Arrange
            var client = CreateDisasterCardClient(HttpStatusCode.OK, "Invalid JSON");

            // Act
            var result = await client.GetByIdAsync(999);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("Deserialization error", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetByIdAsync_WhenDeserializedContentIsNull_ReturnsFailure()
        {
            // Arrange
            var client = CreateDisasterCardClient(HttpStatusCode.OK, "null");

            // Act
            var result = await client.GetByIdAsync(999);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("Deserialized content was null.", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        private static DisasterCardClient CreateDisasterCardClient(HttpStatusCode statusCode, string responseContent = "")
        {
            var stubHandler = new StubHttpMessageHandler(responseContent, statusCode);

            var httpClient = new HttpClient(stubHandler)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            return new DisasterCardClient(httpClient);
        }
    }
}
