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

        public async Task TransferSingleCharacterAsync(string fromLocation, string toLocation)
        {
            var transferButton = Page.GetByTestId($"transfer-{Slugify(fromLocation)}");
            await transferButton.WaitForAsync();
            await Assertions.Expect(transferButton).ToBeEnabledAsync();
            await transferButton.ClickAsync();

            var fromDropdown = Page.Locator("#transfer-destination");
            await fromDropdown.SelectOptionAsync(new SelectOptionValue { Label = toLocation });

            var confirmButton = Page.GetByTestId("confirm-transfer");
            await confirmButton.WaitForAsync();
            await Assertions.Expect(confirmButton).ToBeEnabledAsync();
            await confirmButton.ClickAsync();
        }

        public async Task AssertPageDisplayedAsync()
        {
            await Assertions.Expect(
                    Page.GetByRole(
                        AriaRole.Heading,
                        new()
                        {
                            Name = "Game Dashboard",
                            Exact = true
                        }))
                .ToBeVisibleAsync();

            await Assertions.Expect(
                    Page.GetByRole(AriaRole.Table))
                .ToBeVisibleAsync();
        }

        public async Task AssertThunderbirdLocationAsync(string thunderbirdName, string expectedLocation)
        {
            var row = GetMachineRow(thunderbirdName);
            var locationCell = row.GetByRole(AriaRole.Cell).Nth(0);

            await Assertions.Expect(locationCell)
                .ToHaveTextAsync(expectedLocation);
        }

        public async Task AssertCharacterLocationAsync(string characterName, string thunderbirdCode)
        {
            var row = GetMachineRow(thunderbirdCode);
            var locationCell = row.GetByRole(AriaRole.Cell).Nth(1);
            await Assertions.Expect(locationCell)
                .ToContainTextAsync(characterName);
        }

        public async Task AssertTransferDisabledAsync(string thunderbirdName)
        {
            var transferButton = Page.GetByTestId($"transfer-{Slugify(thunderbirdName)}");
            await Assertions.Expect(transferButton).ToBeDisabledAsync();
        }

        public async Task AssertMoveDisabledAsync(string thunderbirdName)
        {
            var moveButton = Page.GetByTestId($"move-{Slugify(thunderbirdName)}");
            await Assertions.Expect(moveButton).ToBeDisabledAsync();
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
