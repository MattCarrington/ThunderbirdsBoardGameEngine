using Microsoft.Playwright;
using Reqnroll;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Support;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.StepDefinitions
{
    [Binding]
    public class Hooks
    {
        private readonly UiContext _ui;

        public Hooks(UiContext uiContext)
        {
            _ui = uiContext;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _ui.Playwright = await Playwright.CreateAsync();

            _ui.Browser = await _ui.Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            _ui.BrowserContext = await _ui.Browser.NewContextAsync();
            _ui.Page = await _ui.BrowserContext.NewPageAsync();
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            if (_ui.Page is { })
            {
                await _ui.Page.CloseAsync(); 
            }

            if (_ui.BrowserContext is { })
            {
                await _ui.BrowserContext.CloseAsync(); 
            }

            if (_ui.Browser is { })
            {
                await _ui.Browser.CloseAsync(); 
            }

            _ui.Playwright?.Dispose();
        }
    }
}
