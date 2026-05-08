using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Client.Extensions;
using ThunderbirdsBoardGameEngine.Catalog.WireMock;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime;
using ThunderbirdsBoardGameEngine.Rules.Client.Extensions;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.Rules.WireMock;
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
            _host.CharactersStub().RegisterMissingHeaderGuard();
            _host.CharactersStub().RegisterIncorrectHeaderGuard();
            _host.RescueStub().RegisterMissingHeaderGuard();
            _host.RescueStub().RegisterIncorrectHeaderGuard();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "CatalogClient:BaseAddress", _host.Url },
                    { "RulesClient:BaseAddress", _host.Url }
                })
                .Build();

            Services.AddReferenceData();
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

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await
            cut.WaitForElement("#disasterSelect");

            // Assert
            var result = cut
                .FindAll("#disasterSelect option")
                .Select(o => o.TextContent.Trim())
                .ToList();

            Assert.Equal(35, result.Count);
        }

        [Fact]
        public async Task Calculate_WhenSuccess_DisplaysResultAsync()
        {
            // Arrange
            var rescueResult = new CalculateRescueTargetResponseDto
            {
                TargetNumber = 10,
                TotalBonus = 5,
                AppliedDisasterBonuses = Array.Empty<AppliedDisasterBonusDto>()
            };

            _host.CharactersStub().RegisterGetAllSuccess();
            _host.RescueStub().RegisterCalculateRescueTargetSuccess(rescueResult);

            var cut = RenderComponent<DisasterCards>();

            // Wait for initial load
            cut.WaitForElement("#disasterSelect");

            // Act
            cut.Find("#disasterSelect")
               .Change("end-of-the-road");

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
            _host.CharactersStub().RegisterGetAllSuccess();
            _host.RescueStub().RegisterCalculateRescueTargetError();

            var cut = RenderComponent<DisasterCards>();

            // Wait for initial load
            cut.WaitForElement("#disasterSelect");

            // Act - select disaster card
            cut.Find("#disasterSelect").Change("end-of-the-road");

            // Wait for the card details to render (this ensures selectedCard is set)
            cut.WaitForAssertion(() =>
                Assert.Contains("Disaster Card Details", cut.Markup));

            // Now wait for character select (which only shows when selectedCard is not null)
            var characterSelect = cut.WaitForElement("#characterSelect");
            characterSelect.Change("gordon");

            // Click calculate
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert – error appears
            cut.WaitForAssertion(() =>
            {
                cut.Find("[data-testid='rescue-calculation-error']");
            });
        }
    }
}
