using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.Rules.WireMock;
using ThunderbirdsBoardGameEngine.TestUtils.Rules.Factories;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Client.ComponentTests.Clients.V1
{
    [Collection("WireMock")]
    public class RescueClientTests : IAsyncLifetime
    {
        private readonly WireMockHost _host;
        private readonly IRescueClient _client;
        private readonly ServiceProvider _sp;

        public ValueTask InitializeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await _sp.DisposeAsync();
        }

        public RescueClientTests(WireMockFixture fixture)
        {
            _host = fixture.Host;
            _host.Reset();
            _host.RescueStub().RegisterMissingHeaderGuard();
            _host.RescueStub().RegisterIncorrectHeaderGuard();

            _sp = RulesClientProviderFactory.Build(_host.Url!);
            _client = _sp.GetRequiredService<IRescueClient>();
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenValidResponse_ReturnsSuccessApiResult()
        {
            // Arrange
            var dto = new CalculateRescueTargetResponseDto
            {
                AppliedDisasterBonuses = Array.Empty<AppliedDisasterBonusDto>(),
                TargetNumber = 7,
                TotalBonus = 4
            };

            _host.RescueStub().RegisterCalculateRescueTargetSuccess(dto);

            var (disasterCardCode, request) = CreateRequest();

            // Act
            var response = await _client.CalculateRescueTargetAsync(disasterCardCode, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Success);
            Assert.Null(response.ErrorMessage);

            var result = Assert.IsType<CalculateRescueTargetResponseDto>(response.Data);
            Assert.NotNull(result);
            Assert.Equal(dto.TargetNumber, result.TargetNumber);
            Assert.Equal(dto.TotalBonus, result.TotalBonus);
            Assert.Empty(result.AppliedDisasterBonuses);
        }

        [Fact]
        public async Task GetAllAsync_WhenServerError_ReturnsFailureApiResult()
        {
            // Arrange
            _host.RescueStub().RegisterCalculateRescueTargetError();

            var (disasterCardCode, request) = CreateRequest();

            // Act
            var response = await _client.CalculateRescueTargetAsync(disasterCardCode, request, TestContext.Current.CancellationToken);

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
            _host.RescueStub().RegisterCalculateRescueTargetNotFound();

            var (disasterCardCode, request) = CreateRequest();

            // Act
            var response = await _client.CalculateRescueTargetAsync(disasterCardCode, request, TestContext.Current.CancellationToken);

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
            _host.RescueStub().RegisterInvalidCalculateRescueTargetRequest();

            var (disasterCardCode, request) = CreateRequest();

            // Act
            var response = await _client.CalculateRescueTargetAsync(disasterCardCode, request, TestContext.Current.CancellationToken);

            // Assert           
            Assert.False(response.Success);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Null(response.Data);
            Assert.NotNull(response.ErrorMessage);
        }

        private static (string, CalculateRescueTargetRequestDto) CreateRequest()
        {
            var request = new CalculateRescueTargetRequestDto()
            {
                PresentDisasterBonusKeys = ["character:john", "character:gordon"],
                PerformingCharacterKey = "virgil"
            };

            var cardCode = "tower-of-terror";

            return (cardCode, request);
        }
    }
}
