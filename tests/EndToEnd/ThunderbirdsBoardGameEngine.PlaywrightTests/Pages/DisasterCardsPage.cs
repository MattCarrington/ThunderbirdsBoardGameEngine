using Microsoft.Playwright;
using System;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Support;
using Xunit;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.Pages
{
    public class DisasterCardsPage
    {
        private readonly UiContext _ui;
        private readonly TestSettings _settings;

        private IPage Page => _ui.Page;

        private ILocator DisasterCardDropdown => Page.Locator("#disasterSelect");

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
            await DisasterCardDropdown.SelectOptionAsync(new SelectOptionValue { Label = cardName });
        }

        public async Task SelectCharacterAsync(string characterName)
        {
            var dropdown = Page.Locator("#characterSelect");
            await dropdown.SelectOptionAsync(new SelectOptionValue { Label = characterName });
        }

        public async Task MarkBonusCheckboxAsync(string bonusName)
        {
            var checkbox = Page.GetByLabel(bonusName);

            await checkbox.WaitForAsync();

            if (!await checkbox.IsCheckedAsync())
            {
                await checkbox.CheckAsync();
            }
        }

        public async Task ClickCalculateButton()
        {
            var button = Page.GetByTestId("calculate-button");

            await button.WaitForAsync();

            // Guard against async disable during calculation
            await Assertions.Expect(button).ToBeEnabledAsync();

            await button.ClickAsync();
        }

        public async Task AssertHasAnyCardsAsync()
        {
            Assert.True(await DisasterCardDropdown.IsVisibleAsync(),
                "Expected the card dropdown to be visible.");

            var optionTexts = await DisasterCardDropdown.Locator("option").AllInnerTextsAsync();
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

        public async Task AssertRescueResultDisplayedAsync(int expectedTarget)
        {
            var result = Page.GetByTestId("rescue-calculation-result");

            await Assertions.Expect(result).ToBeVisibleAsync();
            await Assertions.Expect(result)
                .ToContainTextAsync($"Target Number: {expectedTarget}");
        }

        public async Task AssertRescueResultDisplayedAsync()
        {
            var result = Page.GetByTestId("rescue-calculation-result");

            await Assertions.Expect(result).ToBeVisibleAsync();
            await Assertions.Expect(result)
                .ToContainTextAsync($"Target Number:");
            await Assertions.Expect(result)
                .ToContainTextAsync($"Total Bonus:");
        }
    }
}
