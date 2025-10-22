using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.V1
{
    public class DisasterCardsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private const int ApiVersion = 1;
        private const string Route = "/api/disaster-cards";

        public DisasterCardsEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact] 
        public async Task GetAllDisasterCards_WhenValidApiVersionHeader_ReturnsExpectedDataset() 
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, Route);
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
        public async Task GetDisasterCards_MissingApiVersionHeader_ReturnsBadRequest()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, Route);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
            Assert.Equal("ApiVersionUnspecified", problem.Title);
            Assert.True(problem.Extensions.ContainsKey("traceId")); 
            Assert.False(string.IsNullOrWhiteSpace(problem.Detail));
        }

        [Fact] 
        public async Task GetDisasterCards_InvalidApiVersionHeader_ReturnsBadRequest()
        { 
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, Route);
            request.Headers.Add("X-API-Version", "99999");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status400BadRequest, problem!.Status);
            Assert.Equal("UnsupportedApiVersion", problem.Title);
            Assert.True(problem.Extensions.ContainsKey("traceId"));

            // When ReportApiVersions = true, library includes supported versions
            Assert.True(response.Headers.Contains("api-supported-versions"));
        }

        [Fact]
        public async Task GetDisasterCards_MultipleApiVersionHeaders_ReturnsBadRequest()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, Route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Headers.Add("X-API-Version", (ApiVersion + 1).ToString());

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status400BadRequest, problem!.Status);
            Assert.Equal("AmbiguousApiVersion", problem.Title);
            Assert.True(problem.Extensions.ContainsKey("traceId"));
        }
    }
}
