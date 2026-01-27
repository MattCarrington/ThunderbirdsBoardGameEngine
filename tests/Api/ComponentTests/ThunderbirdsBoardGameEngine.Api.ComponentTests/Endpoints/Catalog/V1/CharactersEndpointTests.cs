using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.Catalog.V1
{
    public class CharactersEndpointTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        private const int ApiVersion = 1;
        private const string Route = "/api/catalog/characters";

        public CharactersEndpointTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetCharacters_WhenValidApiVersionHeader_ReturnsOk()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, Route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var actual = await response.Content.ReadFromJsonAsync<List<CharacterDto>>();

            Assert.NotNull(actual);

            var expected = new List<CharacterDto>
            {
                new() { Key = "scott", DisplayName = "Scott" },
                new() { Key = "virgil", DisplayName = "Virgil" },
                new() { Key = "john", DisplayName = "John" },
                new() { Key = "gordon", DisplayName = "Gordon" },
                new() { Key = "alan", DisplayName = "Alan" },
                new() { Key = "lady-penelope", DisplayName = "Lady Penelope" }
            };

            Assert.Equal(expected.OrderBy(x => x.Key), actual.OrderBy(x => x.Key));
        }

        [Fact]
        public async Task GetCharacters_MissingApiVersionHeader_ReturnsBadRequest()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, Route);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "ApiVersionUnspecified");
        }

        [Fact]
        public async Task GetCharacters_InvalidApiVersionHeader_ReturnsBadRequest()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, Route);
            request.Headers.Add("X-API-Version", "99999");

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "UnsupportedApiVersion");

            // When ReportApiVersions = true, library includes supported versions
            Assert.True(response.Headers.Contains("api-supported-versions"));
        }

        [Fact]
        public async Task GetCharacters_MultipleApiVersionHeaders_ReturnsBadRequest()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, Route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Headers.Add("X-API-Version", (ApiVersion + 1).ToString());

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "AmbiguousApiVersion");
        }
    }
}
