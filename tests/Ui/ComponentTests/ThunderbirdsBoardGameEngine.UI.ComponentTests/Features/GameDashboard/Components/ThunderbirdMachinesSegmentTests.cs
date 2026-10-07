using Bunit;
using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.GameDashboard.Components
{
    public class ThunderbirdMachinesSegmentTests : BunitContext
    {
        [Fact]
        public void DisplaysMachinesLocationsAndOccupants()
        {
            // Arrange
            var machines = new[]
            {
                GameDashboardViewModelFactory.CreateMachine(
                    thunderbirdCode: "thunderbird-1",
                    thunderbirdDisplayName: "Thunderbird 1",
                    locationCode: "south-pacific",
                    locationDisplayName: "South Pacific",
                    occupants: [
                        GameDashboardViewModelFactory.CreateOccupant(
                            characterCode: "scott",
                            characterDisplayName: "Scott"),
                        GameDashboardViewModelFactory.CreateOccupant(
                            characterCode: "alan",
                            characterDisplayName: "Alan")
                    ]),
                GameDashboardViewModelFactory.CreateMachine(
                    thunderbirdCode: "thunderbird-2",
                    thunderbirdDisplayName: "Thunderbird 2",
                    locationCode: "europe",
                    locationDisplayName: "Europe",
                    occupants: [])
            };

            // Act
            var cut = Render<ThunderbirdMachinesSegment>(
                parameters => parameters
                    .Add(component => component.Machines, machines));

            var rows = cut.FindAll("tbody tr");

            // Assert
            Assert.Collection(
                rows,
                first =>
                {
                    Assert.Contains("Thunderbird 1", first.TextContent);
                    Assert.Contains("South Pacific", first.TextContent);
                    Assert.Contains("Scott, Alan", first.TextContent);
                },
                second =>
                {
                    Assert.Contains("Thunderbird 2", second.TextContent);
                    Assert.Contains("Europe", second.TextContent);
                    Assert.Contains("Empty", second.TextContent);
                });
        }

        [Fact]
        public void InvokesMoveRequestedCallbackWhenMoveButtonClicked()
        {
            // Arrange
            var machines = new[]
            {
                GameDashboardViewModelFactory.CreateMachine(
                    thunderbirdCode: "thunderbird-1",
                    thunderbirdDisplayName: "Thunderbird 1",
                    locationCode: "south-pacific",
                    locationDisplayName: "South Pacific",
                    occupants: []),
                GameDashboardViewModelFactory.CreateMachine(
                    thunderbirdCode: "thunderbird-2",
                    thunderbirdDisplayName: "Thunderbird 2",
                    locationCode: "europe",
                    locationDisplayName: "Europe",
                    occupants: [])
            };

            ThunderbirdMachinesDashboardViewModel? callbackParameter = null;

            var cut = Render<ThunderbirdMachinesSegment>(
                parameters => parameters
                    .Add(component => component.Machines, machines)
                    .Add(component => component.MoveRequested, EventCallback.Factory.Create<ThunderbirdMachinesDashboardViewModel>(this, (machine) =>
                    {
                        callbackParameter = machine;
                    })));

            cut.Find("[data-testid='move-thunderbird-2']").Click();

            // Assert
            Assert.NotNull(callbackParameter);
            Assert.Equal("thunderbird-2", callbackParameter!.ThunderbirdCode);
        }
    }
}
