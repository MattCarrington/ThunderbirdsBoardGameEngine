using AngleSharp.Dom;
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
            var machines = CreateThunderbirds();

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
            var machines = CreateThunderbirds();

            ThunderbirdMachinesDashboardViewModel? callbackParameter = null;

            var cut = Render<ThunderbirdMachinesSegment>(
                parameters => parameters
                    .Add(component => component.Machines, machines)
                    .Add(component => component.MoveRequested, EventCallback.Factory.Create<ThunderbirdMachinesDashboardViewModel>(this, (machine) =>
                    {
                        callbackParameter = machine;
                    })));

            // Act
            cut.Find("[data-testid='move-thunderbird-2']").Click();

            // Assert
            Assert.NotNull(callbackParameter);
            Assert.Equal("thunderbird-2", callbackParameter!.ThunderbirdCode);
        }

        [Fact]
        public void InvokesTransferRequestedCallbackWhenTransferButtonClicked()
        {
            // Arrange
            var machines = CreateThunderbirds();

            ThunderbirdMachinesDashboardViewModel? callbackParameter = null;

            var cut = Render<ThunderbirdMachinesSegment>(
                parameters => parameters
                    .Add(component => component.Machines, machines)
                    .Add(component => component.TransferRequested, EventCallback.Factory.Create<ThunderbirdMachinesDashboardViewModel>(this, (machine) =>
                    {
                        callbackParameter = machine;
                    })));

            // Act
            cut.Find("[data-testid='transfer-thunderbird-1']").Click();

            // Assert
            Assert.NotNull(callbackParameter);
            Assert.Equal("thunderbird-1", callbackParameter!.ThunderbirdCode);
        }

        [Fact]
        public void TransferButtonIsDisabledWhenThereAreNoOccupants()
        {
            // Arrange
            var machines = CreateThunderbirds();

            var emptyMachine = Assert.Single(machines, machine => machine.ThunderbirdCode == "thunderbird-2");

            Assert.Empty(emptyMachine.Occupants);

            var cut = Render<ThunderbirdMachinesSegment>(
                parameters => parameters
                    .Add(component => component.Machines, machines));

            // Act
            var button = cut.Find("[data-testid='transfer-thunderbird-2']");

            // Assert
            Assert.True(button.IsDisabled());
        }

        [Fact]
        public void MoveButtonIsDisabledWhenMachineHasNoOccupants()
        {
            // Arrange
            var machine = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdCode: "thunderbird-4",
                occupants: []);

            var cut = Render<ThunderbirdMachinesSegment>(
                parameters => parameters
                    .Add(component => component.Machines, [machine]));

            // Act
            var button = cut.Find("[data-testid='move-thunderbird-4']");

            // Assert
            Assert.True(button.IsDisabled());
        }

        [Fact]
        public void TransferButtonIsDisabledWhenNoOtherMachineIsCoLocated()
        {
            // Arrange
            var machines = CreateThunderbirds();

            var cut = Render<ThunderbirdMachinesSegment>(
                parameters => parameters
                    .Add(component => component.Machines, machines));

            // Act
            var button = cut.Find("[data-testid='transfer-thunderbird-1']");

            // Assert
            Assert.True(button.IsDisabled());
        }

        private static ThunderbirdMachinesDashboardViewModel[] CreateThunderbirds()
        {
            return
            [
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
            ];
        }
    }
}
