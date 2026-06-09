using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.Rules.WireMock;
using ThunderbirdsBoardGameEngine.TestUtils.Rules.Factories;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Client.ComponentTests.Clients.V1
{
    [Collection("WireMock")]
    public class MovementClientTests : IAsyncLifetime
    {
        private readonly WireMockHost _host;
        private readonly IMovementClient _client;
        private readonly ServiceProvider _sp;

        private const string ThunderbirdCode = "thunderbird-1";

        public ValueTask InitializeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await _sp.DisposeAsync();
        }

        public MovementClientTests(WireMockFixture fixture)
        {
            _host = fixture.Host;
            _host.Reset();
            _host.MovementStub().RegisterMissingHeaderGuard();
            _host.MovementStub().RegisterIncorrectHeaderGuard();

            _sp = RulesClientProviderFactory.Build(_host.Url!);
            _client = _sp.GetRequiredService<IMovementClient>();
        }

        [Fact]
        public async Task ValidateMovementAsync_WhenValidResponse_ReturnsSuccessApiResult()
        {
            // Arrange
            var dto = new ValidateMovementResponseDto()
            {
                IsValid = true,
                ActionPointCost = 2,
                SpacesTravelled = 3,
                TopSpeed = 5,
                Route = ["europe", "middle-east", "asia"],
                Messages = ["Movement is valid."]
            };

            _host.MovementStub().RegisterValidateMovementSuccess(dto);

            var request = CreateRequest();

            // Act
            var response = await _client.ValidateMovementAsync(ThunderbirdCode, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Success);
            Assert.Null(response.ErrorMessage);

            var result = Assert.IsType<ValidateMovementResponseDto>(response.Data);
            Assert.NotNull(result);
            Assert.Equal(dto.IsValid, result.IsValid);
            Assert.Equal(dto.ActionPointCost, result.ActionPointCost);
            Assert.Equal(dto.SpacesTravelled, result.SpacesTravelled);
            Assert.Equal(dto.TopSpeed, result.TopSpeed);
            Assert.Equal(dto.Route, result.Route);
            Assert.Equal(dto.Messages, result.Messages);
        }

        [Fact]
        public async Task GetAllAsync_WhenServerError_ReturnsFailureApiResult()
        {
            // Arrange
            _host.MovementStub().RegisterValidateMovementError();

            var request = CreateRequest();

            // Act
            var response = await _client.ValidateMovementAsync(ThunderbirdCode, request, TestContext.Current.CancellationToken);

            // Assert           
            Assert.False(response.Success);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Null(response.Data);
            Assert.NotNull(response.ErrorMessage);
        }

        [Fact]
        public async Task GetAllAsync_WhenNotFound_ReturnsFailureApiResult()
        {
            // Arrange
            _host.MovementStub().RegisterValidateMovementNotFound();

            var request = CreateRequest();

            // Act
            var response = await _client.ValidateMovementAsync(ThunderbirdCode, request, TestContext.Current.CancellationToken);

            // Assert           
            Assert.False(response.Success);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Null(response.Data);
            Assert.NotNull(response.ErrorMessage);
        }

        [Fact]
        public async Task GetAllAsync_WhenBadRequest_ReturnsFailureApiResult()
        {
            // Arrange
            _host.MovementStub().RegisterInvalidValidateMovementRequest();

            var request = CreateRequest();

            // Act
            var response = await _client.ValidateMovementAsync(ThunderbirdCode, request, TestContext.Current.CancellationToken);

            // Assert           
            Assert.False(response.Success);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Null(response.Data);
            Assert.NotNull(response.ErrorMessage);
        }

        private static ValidateMovementRequestDto CreateRequest()
        {
            var request = new ValidateMovementRequestDto()
            {
                StartLocation = "europe",
                DestinationLocation = "asia"
            };

            return (request);
        }
    }
}
