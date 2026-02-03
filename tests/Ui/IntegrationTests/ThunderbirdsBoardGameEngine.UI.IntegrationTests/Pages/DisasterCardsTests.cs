using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Client.Extensions;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.WireMock;
using ThunderbirdsBoardGameEngine.Rules.Client.Extensions;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.Rules.WireMock;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.TestFileCatalogs;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Pages;
using ThunderbirdsBoardGameEngine.UI.Services;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;
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

            Services.AddSingleton<ICharactersService, CharactersService>();
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

        [Fact]
        public async Task Calculate_WhenSuccess_DisplaysResultAsync()
        {
            // Arrange
            var cards = await GetCardDtosAsync();

            var rescueResult = new CalculateRescueTargetResponseDto
            {
                TargetNumber = 10,
                TotalBonus = 5,
                AppliedDisasterBonuses = Array.Empty<AppliedDisasterBonusDto>()
            };

            _host.DisasterCardStub().RegisterGetAllSuccess(cards);
            _host.CharactersStub().RegisterGetAllSuccess();
            _host.RescueStub().RegisterCalculateRescueTargetSuccess(rescueResult);

            var cut = RenderComponent<DisasterCards>();

            // Wait for initial load
            cut.WaitForElement("#disasterSelect");

            // Act
            cut.Find("#disasterSelect")
               .Change(cards[0].Id.ToString());

            cut.WaitForState(() => true);

            cut.WaitForElement("#characterSelect");

            cut.Find("#characterSelect").Change("gordon");

            // Now click
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert – UI reaction
            cut.WaitForAssertion(() =>
            {
                var result = cut.Find("[data-testid='rescue-calculation-result']");

                Assert.Contains(rescueResult.TargetNumber.ToString(), result.TextContent);
                Assert.Contains(rescueResult.TotalBonus.ToString(), result.TextContent);

                // Error is NOT visible
                Assert.Empty(cut.FindAll("[data-testid='rescue-calculation-error']"));
            });
        }

        [Fact]
        public async Task Calculate_WhenError_DisplaysErrorAsync()
        {
            // Arrange
            var cards = await GetCardDtosAsync();

            _host.DisasterCardStub().RegisterGetAllSuccess(cards);
            _host.CharactersStub().RegisterGetAllSuccess();
            _host.RescueStub().RegisterCalculateRescueTargetError();

            var cut = RenderComponent<DisasterCards>();

            // Wait for initial load
            cut.WaitForElement("#disasterSelect");

            // Act
            cut.Find("#disasterSelect")
               .Change(cards[0].Id.ToString());

            cut.WaitForState(() => true);

            cut.WaitForElement("#characterSelect");

            cut.Find("#characterSelect").Change("gordon");

            // Now click
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert – UI reaction
            cut.WaitForAssertion(() =>
            {
                cut.Find("[data-testid='rescue-calculation-error']");
            });
        }

        private static async Task<IReadOnlyList<DisasterCardDto>> GetCardDtosAsync()
        {
            return await TestDataLoader.LoadJsonFromFileAsync<IReadOnlyList<DisasterCardDto>>(DisasterCardTestFileCatalog.DataOnly("disaster-card-dtos.json"));
        }
    }
}
