using Bunit;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.GameDashboard.Components
{
    public class MoveThunderbirdDialogTests : BunitContext
    {
        [Fact]
        public void MoveThunderbirdDialogShouldRenderCorrectly()
        {
            // Arrange
            var thunderbird = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                locationDisplayName: "Location 1",
                occupants:
                [
                    new("OCC1", "Occupant 1"),
                    new("OCC2", "Occupant 2")
                ]
            );

            // Act
            var cut = Render<MoveThunderbirdDialog>(
                parameters => parameters
                    .Add(p => p.Machine, thunderbird)
                    .Add(p => p.Destinations, new List<LocationOptionsViewModel>
                    {
                        new("LOC2", "Location 2"),
                        new("LOC3", "Location 3")
                    })
            );

            // Assert
            Assert.Contains("Move Thunderbird 1", cut.Markup);
            Assert.Contains("Current location:", cut.Markup);
            Assert.Contains("Location 1", cut.Markup);
            Assert.Contains("Location 2", cut.Markup);
            Assert.Contains("Location 3", cut.Markup);
        }

        [Fact]
        public void SelectingDestinationAndConfirmingRaisesDestinationCode()
        {
            // Arrange
            string? confirmedDestination = null;

            var cut = Render<MoveThunderbirdDialog>(
                parameters => parameters
                    .Add(component => component.Machine, GameDashboardViewModelFactory.CreateMachine())
                    .Add(
                        component => component.Destinations,
                        [new("venus", "Venus")])
                    .Add(
                        component => component.MoveConfirmed,
                        destination => confirmedDestination = destination));

            // Act
            var confirm = cut.Find("[data-testid='confirm-move']");

            Assert.True(confirm.HasAttribute("disabled"));

            cut.Find("#move-destination").Change("venus");

            Assert.False(confirm.HasAttribute("disabled"));

            confirm.Click();

            // Assert
            Assert.Equal("venus", confirmedDestination);
        }

        [Fact]
        public void ClickingCancelRaisesMoveCancelled()
        {
            // Arrange
            bool moveCancelledRaised = false;

            var cut = Render<MoveThunderbirdDialog>(
                parameters => parameters
                    .Add(component => component.Machine, GameDashboardViewModelFactory.CreateMachine())
                    .Add(
                        component => component.Destinations,
                        [new("venus", "Venus")])
                    .Add(
                        component => component.Cancelled,
                        () => moveCancelledRaised = true));

            // Act
            cut.Find("[data-testid='cancel-move']").Click();

            // Assert
            Assert.True(moveCancelledRaised);
        }

        [Fact]
        public void DisplaysErrorMessageWhenErrorOccurs()
        {
            // Arrange
            const string errorMessage = "This move is not permitted.";

            // Act
            var cut = Render<MoveThunderbirdDialog>(
                parameters => parameters
                    .Add(component => component.Machine, GameDashboardViewModelFactory.CreateMachine())
                    .Add(
                        component => component.Destinations,
                        [new("venus", "Venus")])
                    .Add(
                        component => component.ErrorMessage,
                        errorMessage));

            // Assert
            Assert.Contains(errorMessage, cut.Markup);
        }

        [Fact]
        public void DisplaysIsMovingWhilstProcessing()
        {
            // Arrange

            // Act
            var cut = Render<MoveThunderbirdDialog>(
                parameters => parameters
                    .Add(component => component.Machine, GameDashboardViewModelFactory.CreateMachine())
                    .Add(
                        component => component.Destinations,
                        [new("venus", "Venus")])
                    .Add(
                        component => component.IsMoving,
                        true));

            // Assert
            Assert.Contains("Moving…", cut.Markup);
        }

        [Fact]
        public void DisplaysIsLoadingWhilstFetchingDestinations()
        {
            // Arrange

            // Act
            var cut = Render<MoveThunderbirdDialog>(
                parameters => parameters
                    .Add(component => component.Machine, GameDashboardViewModelFactory.CreateMachine())
                    .Add(
                        component => component.Destinations,
                        [new("venus", "Venus")])
                    .Add(
                        component => component.IsLoadingDestinations,
                        true));

            // Assert
            Assert.Contains("Loading destinations…", cut.Markup);
        }

        [Fact]
        public void DisplaysNoDestinationsMessageWhenListIsEmpty()
        {
            // Arrange

            // Act
            var cut = Render<MoveThunderbirdDialog>(
                parameters => parameters
                    .Add(component => component.Machine, GameDashboardViewModelFactory.CreateMachine())
                    .Add(
                        component => component.Destinations,
                        []));

            // Assert
            Assert.Contains("This Thunderbird Machine has no available destinations.", cut.Markup);
        }
    }
}
