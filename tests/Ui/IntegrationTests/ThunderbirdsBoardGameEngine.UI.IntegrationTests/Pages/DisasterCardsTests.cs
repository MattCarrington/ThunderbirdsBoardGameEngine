using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            // Only rescue uses HTTP now
            _host.RescueStub().RegisterMissingHeaderGuard();
            _host.RescueStub().RegisterIncorrectHeaderGuard();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "CatalogClient:BaseAddress", _host.Url },
                    { "RulesClient:BaseAddress", _host.Url }
                })
                .Build();

            // Register reference data runtime (in-memory catalog for disaster cards & characters)
            Services.AddReferenceData();

            // Only register Rules client (for rescue calculations)
            Services.AddRulesClients(configuration);

            // Register UI services
            Services.AddSingleton<ICharacterService, CharacterService>();
            Services.AddSingleton<IDisasterCardService, DisasterCardService>();
            Services.AddSingleton<IRescueService, RescueService>();
        }

        [Fact]
        public void Render_WhenCardsAndCharactersExist_LoadsFromReferenceData()
        {
            // Act
            var cut = RenderComponent<DisasterCards>();

            // Assert - disaster cards are loaded
            var cardOptions = cut
                .FindAll("#disasterSelect option")
                .Select(o => o.TextContent.Trim())
                .ToList();

            Assert.True(cardOptions.Count > 1, "Expected disaster cards to be loaded from reference data");
            Assert.Equal("-- Select a card --", cardOptions[0]);
        }

        [Fact]
        public void Calculate_WhenSuccess_DisplaysResult()
        {
            // Arrange
            var rescueResult = new CalculateRescueTargetResponseDto
            {
                TargetNumber = 10,
                TotalBonus = 5,
                AppliedDisasterBonuses = Array.Empty<AppliedDisasterBonusDto>()
            };

            _host.RescueStub().RegisterCalculateRescueTargetSuccess(rescueResult);

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Select disaster card
            cut.Find("#disasterSelect").Change("end-of-the-road");

            // Wait for card details
            cut.WaitForAssertion(() =>
                Assert.Contains("Disaster Card Details", cut.Markup));

            // Select character
            cut.Find("#characterSelect").Change("gordon");

            // Ensure calculate button is enabled
            cut.WaitForAssertion(() =>
                Assert.False(cut.Find("[data-testid='calculate-button']").HasAttribute("disabled")));

            // Click calculate
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert – result appears (with increased timeout)
            cut.WaitForAssertion(() =>
            {
                // First check that we're not in loading state anymore
                var button = cut.Find("[data-testid='calculate-button']");
                Assert.False(button.HasAttribute("disabled"), "Button should not be disabled after calculation completes");

                // Then check for the result
                var result = cut.Find("[data-testid='rescue-calculation-result']");
                Assert.Contains(rescueResult.TargetNumber.ToString(), result.TextContent);
                Assert.Contains(rescueResult.TotalBonus.ToString(), result.TextContent);

                // Error is NOT visible
                Assert.Empty(cut.FindAll("[data-testid='rescue-calculation-error']"));
            },
            timeout: TimeSpan.FromSeconds(5));  // Increased timeout for WireMock HTTP call
        }

        [Fact]
        public void Calculate_WhenError_DisplaysError()
        {
            // Arrange
            _host.RescueStub().RegisterCalculateRescueTargetError();

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Select disaster card (loads synchronously)
            cut.Find("#disasterSelect").Change("end-of-the-road");

            // Wait for card details (ensures selectedCard is set)
            cut.WaitForAssertion(() =>
                Assert.Contains("Disaster Card Details", cut.Markup));

            // Select character (already loaded synchronously)
            cut.Find("#characterSelect").Change("gordon");

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
