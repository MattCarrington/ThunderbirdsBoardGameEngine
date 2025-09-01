using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Client;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.WireMock;
using ThunderbirdsBoardGameEngine.TestUtils;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Pages;
using ThunderbirdsBoardGameEngine.UI.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.IntegrationTests.Pages
{
    public class DisasterCardsTests : TestContext, IAsyncLifetime
    {
        private WireMockHost _host;

        private readonly IReadOnlyList<DisasterCardDto> _cards = TestDataLoader.LoadJsonFromFile<IReadOnlyList<DisasterCardDto>>("disaster-card-dto-data.json", TestDataConstants.V1InputFolder);

        public Task InitializeAsync()
        {
            _host = new WireMockHost();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "GameDataClient:BaseAddress", _host.Url }
                })
                .Build();

            Services.AddGameDataClients(configuration);
            Services.AddSingleton<IDisasterCardService, DisasterCardService>();

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _host.DisposeAsync();
        }

        [Fact]
        public void Render_WhenCardsExist_CardsExist()
        {
            // Arrange      
            _host.Reset();

            _host.DisasterCardStub.RegisterMissingHeaderGuard();
            _host.DisasterCardStub.RegisterIncorrectHeaderGuard();
            _host.DisasterCardStub.RegisterGetAllSuccess(_cards);

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await
            cut.WaitForElement("#disasterSelect");

            // Assert
            var result = cut
                .FindAll("#disasterSelect option")
                .Select(o => o.TextContent.Trim())
                .ToList();

            Assert.Equal(_cards.Count + 1, result.Count);
        }

        [Fact]
        public void Render_WhenNoCardExist_DisplaysEmptyState()
        {
            // Arrange
            _host.Reset();

            _host.DisasterCardStub.RegisterMissingHeaderGuard();
            _host.DisasterCardStub.RegisterIncorrectHeaderGuard();
            _host.DisasterCardStub.RegisterGetAllSuccess(new List<DisasterCardDto>());

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await
            cut.WaitForElement("[data-testid=empty-state]");

            // Assert
            Assert.Empty(cut.FindAll("#disasterSelect"));
            Assert.DoesNotContain("Disaster Card Details", cut.Markup);

        }

        [Fact]
        public void Render_WhenErrorOccurs_DisplaysEmptyState()
        {
            // Arrange
            _host.Reset();

            _host.DisasterCardStub.RegisterMissingHeaderGuard();
            _host.DisasterCardStub.RegisterIncorrectHeaderGuard();
            _host.DisasterCardStub.RegisterGetAllError(HttpStatusCode.InternalServerError, "An error has occurred");

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await
            cut.WaitForElement("[data-testid=empty-state]");

            // Assert
            Assert.Empty(cut.FindAll("#disasterSelect"));
            Assert.DoesNotContain("Disaster Card Details", cut.Markup);
        }
    }
}
