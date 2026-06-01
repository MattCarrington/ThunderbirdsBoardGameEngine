using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.CrossCutting
{
    public class EndpointVersioningTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        private static readonly string _route = "/api/rules/rescue/danger-at-ocean-deep/target";

        private static readonly CalculateRescueTargetRequestDto _requestDto = new()
        {
            PresentDisasterBonusKeys = [],
            PerformingCharacterKey = "gordon"
        };

        public EndpointVersioningTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task MissingApiVersionHeaderReturnsBadRequestWithTitleUnspecifiedApiVersion()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Content = JsonContent.Create(_requestDto);

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Unspecified API version");
        }

        [Fact]
        public async Task UnsupportedApiVersionHeaderReturnsBadRequestWithTitleUnsupportedApiVersion()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Headers.Add("X-API-Version", "999");
            request.Content = JsonContent.Create(_requestDto);

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Unsupported API version");
        }

        [Fact]
        public async Task MultipleApiVersionHeadersReturnsBadRequestWithTitleAmbiguousApiVersion()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Headers.Add("X-API-Version", "1");
            request.Headers.Add("X-API-Version", "2");
            request.Content = JsonContent.Create(_requestDto);

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Ambiguous API version");
        }

        [Fact]
        public async Task InvalidApiVersionHeaderReturnsBadRequestWithTitleInvalidApiVersion()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Headers.Add("X-API-Version", "invalid");
            request.Content = JsonContent.Create(_requestDto);

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Invalid API version");
        }
    }
}
