using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.Rules.V1
{
    public class MovementEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        private const int ApiVersion = 1;

        public MovementEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ReturnsSuccessWhenRequestIsValid()
        {
            var dto = new ValidateMovementRequestDto
            {
                StartLocation = "south-pacific",
                DestinationLocation = "europe"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/rules/movement/thunderbird-2/validate");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ValidateMovementResponseDto>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(result);
            Assert.True(result.IsValid);
            Assert.Equal(3, result.SpacesTravelled);
            Assert.NotEmpty(result.Route);
            Assert.Equal(2, result.ActionPointCost);
            Assert.Equal(2, result.TopSpeed);
            Assert.Empty(result.Messages);
        }

        [Fact]
        public async Task ReturnsSuccessWhenRequestIsValidButNoRouteFound()
        {
            var dto = new ValidateMovementRequestDto
            {
                StartLocation = "south-pacific",
                DestinationLocation = "the-sun"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/rules/movement/thunderbird-2/validate");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ValidateMovementResponseDto>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Equal(0, result.SpacesTravelled);
            Assert.Empty(result.Route);
            Assert.Equal(0, result.ActionPointCost);
            Assert.Equal(2, result.TopSpeed);

            var message = Assert.Single(result.Messages);
            Assert.Contains("No route found", message);
        }

        [Fact]
        public async Task ReturnsNotFoundWhenThunderbirdDoesNotExist()
        {
            var dto = new ValidateMovementRequestDto
            {
                StartLocation = "south-pacific",
                DestinationLocation = "europe"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/rules/movement/thunderbird-x/validate");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Resource not found.");
        }

        [Fact]
        public async Task ReturnsNotFoundWhenLocationDoesNotExist()
        {
            var dto = new ValidateMovementRequestDto
            {
                StartLocation = "south-pacific",
                DestinationLocation = "north-pole"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/rules/movement/thunderbird-1/validate");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Resource not found.");
        }

        [Fact]
        public async Task ReturnsBadRequestWhenStartLocationNotPresent()
        {
            var dto = new
            {
                DestinationLocation = "north-america"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/rules/movement/thunderbird-1/validate");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
        }

        [Fact]
        public async Task ReturnsBadRequestWhendDestionationLocationNotPresent()
        {
            var dto = new
            {
                StartLocation = "south-america"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/rules/movement/thunderbird-1/validate");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
        }
    }
}
