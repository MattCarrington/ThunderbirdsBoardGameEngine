using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.Rules.V1
{
    public class RescueEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        private const int ApiVersion = 1;
        private const string DisasterCardCode = "they-call-him-mr-x";

        private static readonly string _route = $"/api/rules/rescue/{DisasterCardCode}/target";

        private static readonly CalculateRescueTargetRequestDto _requestDto = new()
        {
            PresentDisasterBonusKeys =
                [
                    "mobile-crane",
                    "domo"
                ],
            PerformingCharacterKey = "gordon"
        };

        public RescueEndpointsTests(WebApplicationFactory<Program> factory)
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
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var target = await response.Content.ReadFromJsonAsync<CalculateRescueTargetResponseDto>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(target);
            Assert.Equal(4, target.TargetNumber);
            Assert.Equal(4, target.TotalBonus);

            var expectedBonuses = new[]
            {
                new AppliedDisasterBonusDto
                {
                    BonusKey = "mobile-crane",
                    BonusValue = 2,
                    SourceType = "disaster-card"
                },
                new AppliedDisasterBonusDto
                {
                    BonusKey = "domo",
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
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var target = await response.Content.ReadFromJsonAsync<CalculateRescueTargetResponseDto>(cancellationToken: TestContext.Current.CancellationToken);

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
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Resource not found.");
        }

        [Fact]
        public async Task CalculateRescueTarget_WhenPerformingCharacterMissing_ReturnsBadRequest()
        {
            // Arrange
            var invalidRequestDto = new
            {
                PresentDisasterBonusKeys = new[]
                {
                    "mobile-crane",
                    "domo"
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(invalidRequestDto);

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            var problem = await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Request validation failed.");
            ProblemDetailsAssertions.AssertValidationErrors(problem, nameof(CalculateRescueTargetRequestDto.PerformingCharacterKey));
        }

        [Fact]
        public async Task CalculateRescueResult_WhenFabCardInvalid_ReturnsBadRequest()
        {
            // Arrange
            var invalidFabCardDto = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = new[]
                {
                    "mobile-crane",
                    "domo"
                },
                PerformingCharacterKey = "gordon",
                PlayedFabCards = new[]
                {
                    "invalid-fab-card"
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, _route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(invalidFabCardDto);

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Invalid rescue calculation request.");
        }
    }
}
