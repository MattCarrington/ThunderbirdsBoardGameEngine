using Microsoft.Playwright;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Support;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.Pages
{
    public class HomePage
    {
        private readonly UiContext _ui;

        private readonly TestSettings _settings;

        private IPage Page => _ui.Page;

        public HomePage(UiContext uiContext, TestSettings testSettings)
        {
            _ui = uiContext ?? throw new ArgumentNullException(nameof(uiContext));
            _settings = testSettings ?? throw new ArgumentNullException(nameof(testSettings));
        }

        public async Task GoToAsync()
        {
            await Page.GotoAsync($"{_settings.BaseUrl}/");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Page.WaitForSelectorAsync("h1:text-is('Thunderbirds Companion')");
        }

        public async Task ClickNewGameAsync()
        {
            var button = Page.GetByTestId("create-game");

            await button.WaitForAsync();

            await Assertions.Expect(button).ToBeEnabledAsync();

            await button.ClickAsync();
        }

        public async Task ResumeGameAsync(Guid gameId)
        {
            var gameIdentifier = Page.GetByLabel("Game identifier");

            await gameIdentifier.FillAsync(gameId.ToString());

            var button = Page.GetByTestId("resume-game");

            await button.WaitForAsync();

            await Assertions.Expect(button).ToBeEnabledAsync();

            await button.ClickAsync();
        }
    }
}
