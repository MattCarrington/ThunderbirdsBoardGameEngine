using Microsoft.Playwright;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.Support
{
    public class UiContext
    {
        public IPlaywright Playwright { get; set; } = null!;

        public IBrowser Browser { get; set; } = null!;

        public IBrowserContext BrowserContext { get; set; } = null!;

        public IPage Page { get; set; } = null!;
    }
}
