using AutoFixture;
using System.Net;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Client.Clients.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Routing.V1;
using ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.Stubs;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.UnitTests.Clients.V1
{
    public class DisasterCardsClientTests
    {
        private readonly Fixture _fixture = new();
        
        public DisasterCardsClientTests()
        {
            _fixture.Customize<DisasterCardDto>(c => c
                .With(d => d.BonusConditions, [.. _fixture.CreateMany<BonusConditionDto>(2)])
                .With(d => d.Rewards, [.. _fixture.CreateMany<RewardDto>(2)]));
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

            var client = new DisasterCardsClient(httpClient);

            // Act
            _ = await client.GetAllAsync();

            // Assert
            Assert.Equal($"http://localhost/{ApiRoutes.DisasterCards}", stubHandler.CapturedRequest?.RequestUri?.ToString());
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

            DisasterCardDtoAssertions.AssertOrderSensitive(disasterCardDtos, result.Data.ToList());
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
        private static DisasterCardsClient CreateDisasterCardClient(HttpStatusCode statusCode, string responseContent = "")
        {
            var stubHandler = new StubHttpMessageHandler(responseContent, statusCode);

            var httpClient = new HttpClient(stubHandler)
            {
                BaseAddress = new Uri("http://example.test")
            };

            return new DisasterCardsClient(httpClient);
        }
    }
}
