using Microsoft.Playwright;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Support;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.Pages
{
    public class GameDashboardPage
    {
        private readonly UiContext _ui;

        private readonly TestSettings _settings;

        private IPage Page => _ui.Page;

        public GameDashboardPage(UiContext uiContext, TestSettings testSettings)
        {
            _ui = uiContext ?? throw new ArgumentNullException(nameof(uiContext));
            _settings = testSettings ?? throw new ArgumentNullException(nameof(testSettings));
        }

        public async Task MoveThunderbirdAsync(string thunderbirdName, string location)
        {
            var moveButton = Page.GetByTestId($"move-{Slugify(thunderbirdName)}");
            await moveButton.WaitForAsync();
            await Assertions.Expect(moveButton).ToBeEnabledAsync();
            await moveButton.ClickAsync();

            var locationDropdown = Page.Locator("#move-destination");
            await locationDropdown.SelectOptionAsync(new SelectOptionValue { Label = location });

            var confirmButton = Page.GetByTestId("confirm-move");
            await confirmButton.WaitForAsync();
            await Assertions.Expect(confirmButton).ToBeEnabledAsync();
            await confirmButton.ClickAsync();
            await AssertThunderbirdLocationAsync(thunderbirdName, location);
        }

        public async Task AssertPageDisplayedAsync()
        {
            await Page.WaitForSelectorAsync("h1:text-is('Game Dashboard')");
            await Assertions.Expect(Page.GetByRole(AriaRole.Table)).ToBeVisibleAsync();
        }

        public async Task AssertThunderbirdLocationAsync(string thunderbirdName, string expectedLocation)
        {
            var row = GetMachineRow(thunderbirdName);
            var locationCell = row.GetByRole(AriaRole.Cell).Nth(0);

            await Assertions.Expect(locationCell)
                .ToHaveTextAsync(expectedLocation);
        }

        private static string Slugify(string input)
        {
            return input.ToLowerInvariant().Replace(" ", "-");
        }

        private ILocator GetMachineRow(string machineName)
        {
            var rowHeader = Page.GetByRole(
                AriaRole.Rowheader,
                new()
                {
                    Name = machineName,
                    Exact = true
                });

            return Page
                .GetByRole(AriaRole.Row)
                .Filter(new LocatorFilterOptions
                {
                    Has = rowHeader
                });
        }
    }
}
