using Microsoft.Playwright;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Support;
using Xunit;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.Pages
{
    public class DisasterCardsPage
    {
        private readonly UiContext _ui;
        private readonly TestSettings _settings;
        
        private IPage Page => _ui.Page;

        private ILocator Dropdown => Page.Locator("#disasterSelect");

        private ILocator DetailsContainer => Page.GetByTestId("details");

        private ILocator CardTitle => DetailsContainer.Locator("h4");

        public DisasterCardsPage(UiContext uiContext, TestSettings settings)
        {
            _ui = uiContext ?? throw new ArgumentNullException(nameof(uiContext));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task GotoAsync()
        {
            await Page.GotoAsync($"{_settings.BaseUrl}/disasterCards");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForSelectorAsync("h3:text-is('Select a Disaster Card')");
        }

        public async Task SelectCardAsync(string cardName)
        {
            await Dropdown.SelectOptionAsync(new SelectOptionValue { Label = cardName });
        }

        public async Task AssertHasAnyCardsAsync()
        {
            Assert.True(await Dropdown.IsVisibleAsync(),
                "Expected the card dropdown to be visible.");

            var optionTexts = await Dropdown.Locator("option").AllInnerTextsAsync();
            var realCards = optionTexts.Where(t => !t.StartsWith("--")).ToList();

            Assert.NotEmpty(realCards);
        }

        public async Task AssertCardDetailsVisibleAsync(string expectedCardName)
        {
            await Assertions.Expect(DetailsContainer).ToBeVisibleAsync();
            await Assertions.Expect(CardTitle).ToHaveTextAsync(expectedCardName);

            await Assertions.Expect(DetailsContainer.GetByText("Difficulty:")).ToBeVisibleAsync();
            await Assertions.Expect(DetailsContainer.GetByText("Location:")).ToBeVisibleAsync();
            await Assertions.Expect(DetailsContainer.GetByText("Rescue Type:")).ToBeVisibleAsync();
        }
    }
}
