using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Client.Extensions;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.WireMock;
using ThunderbirdsBoardGameEngine.Rules.Client.Extensions;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.TestFileCatalogs;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Pages;
using ThunderbirdsBoardGameEngine.UI.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.IntegrationTests.Pages
{
    [Collection("WireMock")]
    public class DisasterCardsTests : TestContext
    {
        private readonly WireMockHost _host;

        public DisasterCardsTests(WireMockFixture fixture)
        {
            _host = fixture.Host;
            _host.Reset();
            _host.DisasterCardStub().RegisterMissingHeaderGuard();
            _host.DisasterCardStub().RegisterIncorrectHeaderGuard();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "CatalogClient:BaseAddress", _host.Url }, 
                    { "RulesClient:BaseAddress", _host.Url }
                })
                .Build();

            Services.AddCatalogClients(configuration);
            Services.AddRulesClients(configuration);

            Services.AddSingleton<IDisasterCardService, DisasterCardService>();
            Services.AddSingleton<IRescueService, RescueService>();
        }

        [Fact]
        public async Task Render_WhenCardsExist_CardsExist()
        {
            // Arrange
            var cards = await GetCardDtosAsync();

            _host.DisasterCardStub().RegisterGetAllSuccess(cards);

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await
            cut.WaitForElement("#disasterSelect");

            // Assert
            var result = cut
                .FindAll("#disasterSelect option")
                .Select(o => o.TextContent.Trim())
                .ToList();

            Assert.Equal(cards.Count + 1, result.Count);
        }

        [Fact]
        public void Render_WhenNoCardExist_DisplaysEmptyState()
        {
            // Arrange
            _host.DisasterCardStub().RegisterGetAllEmpty();

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await & Assert
            cut.WaitForAssertion(() =>
            {
                cut.Find("[data-testid='empty-state']");
                Assert.Empty(cut.FindAll("#disasterSelect"));
                Assert.DoesNotContain("Disaster Card Details", cut.Markup);
            }, timeout: TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Render_WhenErrorOccurs_DisplaysEmptyState()
        {
            // Arrange
            _host.DisasterCardStub().RegisterGetAllError();

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await & Assert
            cut.WaitForAssertion(() =>
            {
                cut.Find("[data-testid='empty-state']");
                Assert.Empty(cut.FindAll("#disasterSelect"));
                Assert.DoesNotContain("Disaster Card Details", cut.Markup);
            }, timeout: TimeSpan.FromSeconds(5));
        }

        private async Task<IReadOnlyList<DisasterCardDto>> GetCardDtosAsync()
        {
            return await TestDataLoader.LoadJsonFromFileAsync<IReadOnlyList<DisasterCardDto>>(DisasterCardTestFileCatalog.DataOnly("disaster-card-dtos.json"));
        }
    }
}
