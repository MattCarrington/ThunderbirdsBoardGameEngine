using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Characters;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.Rules.V1
{
    public class RescueEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        private const int ApiVersion = 1;
        private const string DisasterCardCode = "they-call-him-mr-x";

        private static readonly string _route = $"/api/rules/rescue/{DisasterCardCode}/target";

        private static readonly CalculateRescueTargetRequestDto _requestDto = new()
        {
            PresentDisasterBonusKeys =
                [
                    "podvehicle:mobilecrane",
                    "podvehicle:domo"
                ],
            PerformingCharacterKey = "gordon"
        };

        public RescueEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CalculateRescueTarget_WhenCalled_ReturnsOk()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(_requestDto);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var target = await response.Content.ReadFromJsonAsync<CalculateRescueTargetResponseDto>();

            Assert.NotNull(target);
            Assert.Equal(4, target.TargetNumber);
            Assert.Equal(4, target.TotalBonus);

            var expectedBonuses = new[]
            {
                new AppliedDisasterBonusDto
                {
                    BonusKey = "podvehicle:mobilecrane",
                    BonusValue = 2,
                    SourceType = "disaster-card"
                },
                new AppliedDisasterBonusDto
                {
                    BonusKey = "podvehicle:domo",
                    BonusValue = 2,
                    SourceType = "disaster-card"
                }
            };

            Assert.All(target.AppliedDisasterBonuses, bonus =>
            {
                Assert.Contains(expectedBonuses, expectedBonus =>
                    expectedBonus.BonusKey == bonus.BonusKey &&
                    expectedBonus.BonusValue == bonus.BonusValue);
            });
        }

        [Fact]
        public async Task CalculateRescueTarget_WhenEmptyAppliedBonusKeys_ReturnsOk()
        {
            // Arrange
            var requestDto = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = [],
                PerformingCharacterKey = "virgil"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(requestDto);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var target = await response.Content.ReadFromJsonAsync<CalculateRescueTargetResponseDto>();

            Assert.NotNull(target);
            Assert.Equal(8, target.TargetNumber);
            Assert.Equal(0, target.TotalBonus);
            Assert.Empty(target.AppliedDisasterBonuses);
        }

        [Fact]
        public async Task CalculateRescueTarget_WhenCardDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var route = $"/api/rules/rescue/card-does-not-exist/target";

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(_requestDto);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Resource not found.");
        }

        [Fact]
        public async Task CalculateRescueTarget_WhenAppliedBonusKeysMissing_ReturnsBadRequest()
        {
            // Arrange
            var invalidRequestDto = new
            {
                PerformingCharacterKey = "gordon"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(invalidRequestDto);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            var problem = await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
            ProblemDetailsAssertions.AssertValidationErrors(problem, nameof(CalculateRescueTargetRequestDto.PresentDisasterBonusKeys));
        }

        [Fact]
        public async Task CalculateRescueTarget_WhenPerformingCharacterMissing_ReturnsBadRequest()
        {
            // Arrange
            var invalidRequestDto = new
            {
                PresentDisasterBonusKeys = new[]
                {
                    "podvehicle:mobilecrane",
                    "podvehicle:domo"
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(invalidRequestDto);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            var problem = await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
            ProblemDetailsAssertions.AssertValidationErrors(problem, nameof(CalculateRescueTargetRequestDto.PerformingCharacterKey));
        }

        [Fact]
        public async Task CalculateRescueTarget_WhenPerformingCharacterInvalid_ReturnsBadRequest()
        {
            // Arrange
            var invalidRequestDto = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = new[]
                {
                    "podvehicle:mobilecrane",
                    "podvehicle:domo"
                },
                PerformingCharacterKey = "invalid-character"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(invalidRequestDto);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Bad request.");
        }

        [Fact]
        public async Task CalculateTargetResult_WhenMissingVersionHeader_ReturnsBadRequest()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Content = JsonContent.Create(_requestDto);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "ApiVersionUnspecified");
        }

        [Fact]
        public async Task CalculateRescueTarget_WhenInvalidVersionHeader_ReturnsBadRequest()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Headers.Add("X-API-Version", "999");
            request.Content = JsonContent.Create(_requestDto);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "UnsupportedApiVersion");
        }

        [Fact]
        public async Task CalculateRescueTarget_MultipleApiVersionHeaders_ReturnsBadRequest()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Headers.Add("X-API-Version", (ApiVersion + 1).ToString());
            request.Content = JsonContent.Create(_requestDto);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "AmbiguousApiVersion");
        }
    }
}
