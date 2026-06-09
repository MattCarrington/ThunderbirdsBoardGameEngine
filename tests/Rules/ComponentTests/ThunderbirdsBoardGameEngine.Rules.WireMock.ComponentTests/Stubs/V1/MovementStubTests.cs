using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.Rules.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures;
using WireMock.Server;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.WireMock.ComponentTests.Stubs.V1
{
    [Collection("WireMock")]
    public class MovementStubTests
    {
        private readonly WireMockServer _server;
        private readonly MovementStub _stub;

        public MovementStubTests(WireMockFixture fixture)
        {
            _server = fixture.Host.WireMockServer;
            _server.Reset();

            _stub = fixture.Host.MovementStub();
            _stub.RegisterMissingHeaderGuard();
            _stub.RegisterIncorrectHeaderGuard();
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenSuccessSpecified_ReturnsSuccess()
        {
            // Arrange
            var dto = CreateValidateMovementResponse();

            _stub.RegisterValidateMovementSuccess(dto);

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(MovementStub.Route, CreateValidateMovementRequest(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ValidateMovementResponseDto>(cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(result);
            Assert.Equal(dto.IsValid, result.IsValid);
            Assert.Equal(dto.ActionPointCost, result.ActionPointCost);
            Assert.Equal(dto.SpacesTravelled, result.SpacesTravelled);
            Assert.Equal(dto.TopSpeed, result.TopSpeed);
            Assert.Equal(dto.Route, result.Route);
            Assert.Equal(dto.Messages, result.Messages);
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenNotFoundSpecified_ReturnsNotFound()
        {
            // Arrange
            _stub.RegisterValidateMovementNotFound();

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(MovementStub.Route, CreateValidateMovementRequest(), TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(problem);
            Assert.Contains("Thunderbird", problem.Detail);
            Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
            Assert.Equal("Resource not found", problem.Title);
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenBadRequestSpecified_ReturnsNotFound()
        {
            // Arrange
            _stub.RegisterInvalidValidateMovementRequest();

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(MovementStub.Route, CreateValidateMovementRequest(), TestContext.Current.CancellationToken);

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
            _stub.RegisterValidateMovementError();

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(MovementStub.Route, CreateValidateMovementRequest(), TestContext.Current.CancellationToken);

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

            _stub.RegisterValidateMovementError(HttpStatusCode.ServiceUnavailable, errorMessage);

            using var client = CreateClient();

            // Act
            using var response = await client.PostAsJsonAsync(MovementStub.Route, CreateValidateMovementRequest(), TestContext.Current.CancellationToken);

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

            var dto = CreateValidateMovementResponse();

            _stub.RegisterValidateMovementSuccess(dto); // should not be called

            // Act
            using var response = await client.PostAsJsonAsync(MovementStub.Route, CreateValidateMovementRequest(), TestContext.Current.CancellationToken);

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
            var dto = CreateValidateMovementResponse();

            _stub.RegisterValidateMovementSuccess(dto); // should not be called

            using var client = CreateClient("2.0");

            // Act
            using var response = await client.PostAsJsonAsync(MovementStub.Route, CreateValidateMovementRequest(), TestContext.Current.CancellationToken);

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
            client.DefaultRequestHeaders.Add(MovementStub.VersionHeader, versionHeader);

            return client;
        }

        private HttpClient CreateClient()
        {
            return CreateClient(MovementStub.VersionValue);
        }

        private static ValidateMovementRequestDto CreateValidateMovementRequest()
        {
            return new ValidateMovementRequestDto
            {
                StartLocation = "HQ",
                DestinationLocation = "City",
            };
        }

        private static ValidateMovementResponseDto CreateValidateMovementResponse()
        {
            return new ValidateMovementResponseDto
            {
                IsValid = true,
                ActionPointCost = 3,
                SpacesTravelled = 2,
                TopSpeed = 120,
                Route = ["HQ", "City"],
                Messages = ["Movement is valid."]
            };
        }
    }
}
