using Microsoft.Playwright;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Support;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.Pages
{
    public class MovementPage
    {
        private readonly UiContext _ui;
        private readonly TestSettings _settings;

        private IPage Page => _ui.Page;

        private ILocator ThunderbirdDropdown => Page.Locator("#thunderbirdSelector");

        private ILocator StartLocationDropdown => Page.Locator("#startLocation");

        private ILocator DestinationDropdown => Page.Locator("#destination");

        public MovementPage(UiContext uiContext, TestSettings testSettings)
        {
            _ui = uiContext ?? throw new ArgumentNullException(nameof(uiContext));
            _settings = testSettings ?? throw new ArgumentNullException(nameof(testSettings));
        }

        public async Task GotoAsync()
        {
            await Page.GotoAsync($"{_settings.BaseUrl}/movement");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForSelectorAsync("h3:text-is('Movement')");
        }

        public async Task SelectThunderbirdAsync(string thunderbirdName)
        {
            await ThunderbirdDropdown.SelectOptionAsync(new SelectOptionValue { Label = thunderbirdName });
        }

        public async Task SelectStartLocationAsync(string locationName)
        {
            await StartLocationDropdown.SelectOptionAsync(new SelectOptionValue { Label = locationName });
        }

        public async Task SelectDestinationAsync(string locationName)
        {
            await DestinationDropdown.SelectOptionAsync(new SelectOptionValue { Label = locationName });
        }

        public async Task MarkEventCardCheckboxAsync(string cardName)
        {
            var checkbox = Page.GetByLabel(cardName);

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

        public async Task AssertValidationSuccessDisplayed()
        {
            var success = Page.GetByTestId("validation-success");
            await Assertions.Expect(success).ToBeVisibleAsync();
        }

        public async Task AssertValidationFailureDisplayed()
        {
            var failure = Page.GetByTestId("validation-failure");
            await Assertions.Expect(failure).ToBeVisibleAsync();
        }
    }
}
