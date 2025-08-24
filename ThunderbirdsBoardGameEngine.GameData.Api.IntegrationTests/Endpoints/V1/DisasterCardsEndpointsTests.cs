using System.Net;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.IntegrationTests.Endpoints.V1
{
    public class DisasterCardsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private const int ApiVersion = 1;

        public DisasterCardsEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact] 
        public async Task GetAllDisasterCards_ReturnsExpectedDataset() 
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/DisasterCard");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var cards = JsonSerializer.Deserialize<List<DisasterCardDto>>(json, _jsonOptions);

            Assert.NotNull(cards);

            var expected = TestDataLoader.LoadJsonFromFile<List<DisasterCardDto>>("disaster-card-dtos.json")
                ?? throw new InvalidOperationException("Failed to load expected data.");

            DisasterCardDtoAssertions.AssertOrderInsensitive(expected, cards);

            Assert.Distinct(cards.Select(c => c.Id)); // Assert each card has a unique ID
        }

        [Fact] 
        public async Task GetDisasterCardById_ValidId_ReturnsOkAndExpectedData() 
        {
            // Arrange
            var id = 4;

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/DisasterCard/{id}");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var card = JsonSerializer.Deserialize<DisasterCardDto>(json, _jsonOptions);

            Assert.NotNull(card);

            var expected = TestDataLoader.LoadJsonFromFile<List<DisasterCardDto>>("disaster-card-dtos.json")
                .FirstOrDefault(x => x.Id == 4) 
                    ?? throw new InvalidOperationException($"Expected disaster card with ID {id} not found in expected data.");

            DisasterCardDtoAssertions.AssertEqual(expected, card);
        }

        [Fact] 
        public async Task GetDisasterCardById_InvalidId_ReturnsNotFound()
        { 
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/DisasterCard/9999");
            request.Headers.Add("X-API-Version", "1");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            
            Assert.Contains("Disaster card with ID 9999 not found.", content);
        }

        [Fact] 
        public async Task GetDisasterCards_MissingApiVersionHeader_ReturnsBadRequest()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/DisasterCard");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();

            Assert.Contains("An API version is required, but was not specified.", content);
        }

        [Fact] 
        public async Task GetDisasterCards_InvalidApiVersionHeader_ReturnsBadRequest()
        { 
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/DisasterCard");
            request.Headers.Add("X-API-Version", "99999");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            
            Assert.Contains("does not support the API version '99999'", content);
        }
    }
}
