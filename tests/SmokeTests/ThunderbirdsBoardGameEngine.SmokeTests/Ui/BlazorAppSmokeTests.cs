using Microsoft.Playwright;
using Xunit;

namespace ThunderbirdsBoardGameEngine.SmokeTests.Ui
{
    public class BlazorAppSmokeTests : IAsyncLifetime
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IPage? _page;

        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });
            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();
        }

        [Fact]
        public async Task BlazorAppLoads()
        {
            // Act
            await _page!.GotoAsync(SmokeTestConfig.SmokeTestBaseUrl);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Assert - Just verify the Blazor app shell loaded
            var heading = _page.Locator("h1").First;
            await Assertions.Expect(heading).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
            {
                Timeout = 10000 // Give WASM time to load
            });
        }

        public async Task DisposeAsync()
        {
            if (_page != null)
            {
                await _page.CloseAsync();
            }

            if (_browser != null)
            {
                await _browser.CloseAsync();
            }

            _playwright?.Dispose();
        }
    }
}