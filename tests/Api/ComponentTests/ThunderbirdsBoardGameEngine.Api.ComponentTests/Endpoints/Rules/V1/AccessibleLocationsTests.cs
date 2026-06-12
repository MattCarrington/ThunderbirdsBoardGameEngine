using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.AccessibleLocations.V1;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.Rules.V1
{
    public class AccessibleLocationsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        private const int ApiVersion = 1;

        public AccessibleLocationsTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("thunderbird-1")]
        [InlineData("thunderbird-3")]
        public async Task ReturnsSuccessWhenRequestIsValid(string thunderbirdCode)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/rules/movement/{thunderbirdCode}/accessible-locations");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<AccessibleLocationsResponseDto>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(result);
            Assert.NotEmpty(result.AccessibleLocations);
        }

        [Fact]
        public async Task ReturnsNotFoundWhenThunderbirdDoesNotExist()

        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/rules/movement/thunderbird-x/accessible-locations");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Resource not found.");
        }
    }
}
