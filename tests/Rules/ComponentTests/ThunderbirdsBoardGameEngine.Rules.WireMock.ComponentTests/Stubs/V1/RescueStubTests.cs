using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.AccessibleLocations.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.DetermineFabCardBonus.V1;
using ThunderbirdsBoardGameEngine.Rules.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.TestUtils.WireMock.xUnit.Fixtures;
using WireMock.Server;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.WireMock.ComponentTests.Stubs.V1
{
    [Collection("WireMock")]
    public class RescueStubTests
    {
        private readonly WireMockServer _server;
        private readonly RescueStub _stub;

        public RescueStubTests(WireMockFixture fixture)
        {
            _server = fixture.Host.WireMockServer;
            _server.Reset();

            _stub = fixture.Host.RescueStub();
            _stub.RegisterMissingHeaderGuard();
            _stub.RegisterIncorrectHeaderGuard();
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenSuccessSpecified_ReturnsSuccess()
        {
            // Arrange
            var dto = CreateValidRescueTargetResponse();

            _stub.RegisterCalculateRescueTargetSuccess(dto);

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(RescueStub.CalculateRescueResultRoute, CreateValidRescueTargetRequest(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<CalculateRescueTargetResponseDto>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(result);
            Assert.Equal(dto.TotalBonus, result.TotalBonus);
            Assert.Equal(dto.TargetNumber, result.TargetNumber);
            Assert.Equal(dto.AppliedDisasterBonuses.Count, result.AppliedDisasterBonuses.Count);
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenNotFoundSpecified_ReturnsNotFound()
        {
            // Arrange
            _stub.RegisterCalculateRescueTargetNotFound();

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(RescueStub.CalculateRescueResultRoute, CreateValidRescueTargetRequest(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Contains("disaster card", problem.Detail);
            Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
            Assert.Equal("Resource not found", problem.Title);
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenBadRequestSpecified_ReturnsNotFound()
        {
            // Arrange
            _stub.RegisterInvalidCalculateRescueTargetRequest();

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(RescueStub.CalculateRescueResultRoute, CreateValidRescueTargetRequest(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
            Assert.Contains("Invalid request", problem.Title);
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenErrorUnpecified_ReturnsError()
        {
            // Arrange
            _stub.RegisterCalculateRescueTargetError();

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(RescueStub.CalculateRescueResultRoute, CreateValidRescueTargetRequest(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status500InternalServerError, problem.Status);
            Assert.Equal("An unexpected error occurred.", problem.Title);
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenErrorSpecified_ReturnsError()
        {
            // Arrange
            var errorMessage = "The service is unavailable right now";

            _stub.RegisterCalculateRescueTargetError(HttpStatusCode.ServiceUnavailable, errorMessage);

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(RescueStub.CalculateRescueResultRoute, CreateValidRescueTargetRequest(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, problem.Status);
            Assert.Equal(errorMessage, problem.Title);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            using var client = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };

            var dto = CreateValidRescueTargetResponse();

            _stub.RegisterCalculateRescueTargetSuccess(dto); // should not be called

            // Act
            using var response = await client.PostAsJsonAsync(RescueStub.CalculateRescueResultRoute, CreateValidRescueTargetRequest(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Contains("Missing header", problem.Detail);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
            Assert.Equal("Missing API version header", problem.Title);
        }

        [Fact]
        public async Task GetAllAsync_WhenWrongVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            var dto = CreateValidRescueTargetResponse();

            _stub.RegisterCalculateRescueTargetSuccess(dto); // should not be called

            using var client = CreateClient("2.0");

            // Act
            using var response = await client.PostAsJsonAsync(RescueStub.CalculateRescueResultRoute, CreateValidRescueTargetRequest(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
            Assert.Contains("Unsupported version in header", problem.Title);
        }

        [Fact]
        public async Task GetFabCardBonus_WhenSuccessSpecified_ReturnsSuccess()
        {
            // Arrange
            var dto = CreateValidFabCardBonusResponse();

            _stub.RegisterGetFabCardBonusSuccess(dto);

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(RescueStub.GetFabCardsBonusRoute, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<FabCardBonusResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.Equal(dto.FabCardCode, result.FabCardCode);
            Assert.Equal(dto.BonusValue, result.BonusValue);
        }

        [Fact]
        public async Task GetFabCardBonus_WhenErrorUnspecified_ReturnsError()
        {
            // Arrange
            _stub.RegisterGetFabCardBonusError();

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(RescueStub.GetFabCardsBonusRoute, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status500InternalServerError, problem.Status);
            Assert.Equal("An unexpected error occurred.", problem.Title);
        }

        [Fact]
        public async Task GetFabCardBonus_WhenErrorSpecified_ReturnsError()
        {
            // Arrange
            var errorMessage = "The service is unavailable right now";

            _stub.RegisterGetFabCardBonusError(HttpStatusCode.ServiceUnavailable, errorMessage);

            using var client = CreateClient();

            // Act
            using var response = await client.GetAsync(RescueStub.GetFabCardsBonusRoute, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, problem.Status);
            Assert.Equal(errorMessage, problem.Title);
        }

        [Fact]
        public async Task GetFabCardBonus_WhenNoVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            using var client = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };

            var dto = CreateValidFabCardBonusResponse();

            _stub.RegisterGetFabCardBonusSuccess(dto); // should not be called

            // Act
            using var response = await client.GetAsync(RescueStub.GetFabCardsBonusRoute, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Contains("Missing header", problem.Detail);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
            Assert.Equal("Missing API version header", problem.Title);
        }

        [Fact]
        public async Task GetFabCardBonus_WhenWrongVersionHeader_ReturnsBadRequestAsync()
        {
            // Arrange
            var dto = CreateValidFabCardBonusResponse();

            _stub.RegisterGetFabCardBonusSuccess(dto); // should not be called

            using var client = CreateClient("2.0");

            // Act
            using var response = await client.GetAsync(RescueStub.GetFabCardsBonusRoute, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
            Assert.Contains("Unsupported version in header", problem.Title);
        }

        private HttpClient CreateClient(string versionHeader)
        {
            var client = new HttpClient { BaseAddress = new Uri(_server.Urls[0]) };
            client.DefaultRequestHeaders.Add(RescueStub.VersionHeader, versionHeader);

            return client;
        }

        private HttpClient CreateClient()
        {
            return CreateClient(RescueStub.VersionValue);
        }

        private static CalculateRescueTargetRequestDto CreateValidRescueTargetRequest()
        {
            return new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = [],
                PerformingCharacterKey = "scott"
            };
        }

        private static CalculateRescueTargetResponseDto CreateValidRescueTargetResponse()
        {
            return new CalculateRescueTargetResponseDto
            {
                TargetNumber = 10,
                TotalBonus = 2,
                AppliedDisasterBonuses =
                [
                    new()
                    {
                        BonusKey = "SampleBonusKey",
                        BonusValue = 1,
                        SourceType = "disaster-card"
                    },
                    new()
                    {
                        BonusKey = "AnotherSampleBonusKey",
                        BonusValue = 1,
                        SourceType = "disaster-card"
                    }
                ]
            };
        }

        private static FabCardBonusResponseDto CreateValidFabCardBonusResponse()
        {
            return new FabCardBonusResponseDto
            {
                FabCardCode = "FAB-001",
                BonusValue = 5
            };
        }
    }
}
