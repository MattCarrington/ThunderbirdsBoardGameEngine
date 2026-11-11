using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using Bunit;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.Components;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.GameDashboard.Components
{
    public class TransferCharactersDialogTests : BunitContext
    {
        public TransferCharactersDialogTests()
        {
            DialogJsInterop.Register(this);
        }

        [Fact]
        public void SingleCharacterInSourceDisplayedWithoutCharacterSelector()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                occupants:
                [
                    new("OCC1", "Occupant 1")
                ]
            );

            var destination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 2",
                thunderbirdCode: "thunderbird-2",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                occupants: []
            );

            // Act
            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [destination])
            );

            // Assert
            Assert.Contains("Occupant 1", cut.Markup);
            Assert.Empty(cut.FindComponents<CharacterSelector>());
        }

        [Fact]
        public void MultipleCharactersRenderCharacterSelector()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                occupants:
                [
                    new("OCC1", "Occupant 1"),
                    new("OCC2", "Occupant 2")
                ]
            );

            var destination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 2",
                thunderbirdCode: "thunderbird-2",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                occupants: []
            );

            // Act
            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [destination])
            );

            // Assert
            var selector = cut.FindComponent<CharacterSelector>();
            Assert.Equal(2, selector.Instance.Characters.Count);
            Assert.Contains("Occupant 1", cut.Markup);
            Assert.Contains("Occupant 2", cut.Markup);
        }

        [Fact]
        public void SingleDestinationIsDisplayedWithoutThunderbirdSelector()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                occupants:
                [
                    new("OCC1", "Occupant 1"),
                    new("OCC2", "Occupant 2")
                ]
            );

            var destination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 2",
                thunderbirdCode: "thunderbird-2",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                occupants: []
            );

            // Act
            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [destination])
            );

            // Assert
            Assert.Contains("Thunderbird 2", cut.Markup);
            Assert.Empty(cut.FindComponents<ThunderbirdSelector>());
        }

        [Fact]
        public void MultipleDestinationsRenderThunderbirdSelector()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                occupants:
                [
                    new("OCC1", "Occupant 1"),
                    new("OCC2", "Occupant 2")
                ]
            );

            var destinationOne = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 2",
                thunderbirdCode: "thunderbird-2",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                occupants: []
            );

            var destinationTwo = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 3",
                thunderbirdCode: "thunderbird-3",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                occupants: []
            );

            // Act
            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [destinationOne, destinationTwo])
            );

            // Assert
            var selector = cut.FindComponent<ThunderbirdSelector>();
            Assert.Equal(2, selector.Instance.Thunderbirds.Count);
            Assert.Contains("Thunderbird 2", cut.Markup);
            Assert.Contains("Thunderbird 3", cut.Markup);
        }

        [Fact]
        public void DoesNotDisplayOperationModesWhenNoDestinationIsAvailable()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                occupants:
                [
                    new("OCC1", "Occupant 1"),
                    new("OCC2", "Occupant 2")
                ]
            );

            // Act
            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [])
            );

            // Assert
            Assert.DoesNotContain("Transfer Mode", cut.Markup);
            Assert.Empty(cut.FindAll("[name='transfer-mode']"));
        }

        [Fact]
        public void AllowsOnlyTransferWhenEmptyDestination()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                occupants:
                [
                    new("OCC1", "Occupant 1"),
                    new("OCC2", "Occupant 2")
                ]
            );

            var destination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 2",
                thunderbirdCode: "thunderbird-2",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                occupants: []
            );

            // Act
            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [destination])
            );

            // Assert
            var transferRadio = cut.Find("[data-testid='transfer-mode-transfer']");
            Assert.False(transferRadio.IsDisabled());
            Assert.True(transferRadio.IsChecked());

            var swapRadio = cut.Find("[data-testid='transfer-mode-swap']");
            Assert.True(swapRadio.IsDisabled());
            Assert.False(swapRadio.IsChecked());
        }

        [Fact]
        public void AllowsOnlySwapWhenDestinationIsFullyOccupied()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC1", "Occupant 1"),
                    new("OCC2", "Occupant 2")
                ]
            );

            var destination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 2",
                thunderbirdCode: "thunderbird-2",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 1,
                occupants:
                [
                    new("OCC3", "Occupant 3")
                ]
            );

            // Act
            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [destination])
            );

            // Assert
            var transferRadio = cut.Find("[data-testid='transfer-mode-transfer']");
            Assert.True(transferRadio.IsDisabled());
            Assert.False(transferRadio.IsChecked());

            var swapRadio = cut.Find("[data-testid='transfer-mode-swap']");
            Assert.False(swapRadio.IsDisabled());
            Assert.True(swapRadio.IsChecked());
        }

        [Fact]
        public void AllowsBothTransferAndSwapWhenDestinationHasSpace()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC1", "Occupant 1"),
                    new("OCC2", "Occupant 2")
                ]
            );

            var destination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 2",
                thunderbirdCode: "thunderbird-2",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC3", "Occupant 3")
                ]
            );

            // Act
            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [destination])
            );

            // Assert
            var transferRadio = cut.Find("[data-testid='transfer-mode-transfer']");
            Assert.False(transferRadio.IsDisabled());
            Assert.True(transferRadio.IsChecked());

            var swapRadio = cut.Find("[data-testid='transfer-mode-swap']");
            Assert.False(swapRadio.IsDisabled());
            Assert.False(swapRadio.IsChecked());
        }

        [Fact]
        public void CanSwitchBetweenTransferAndSwapWhenBothAreAvailable()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC1", "Occupant 1"),
                    new("OCC2", "Occupant 2")
                ]
            );

            var destination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 2",
                thunderbirdCode: "thunderbird-2",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC3", "Occupant 3")
                ]
            );

            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [destination])
            );

            // Act
            var swapRadio = cut.Find("[data-testid='transfer-mode-swap']");
            swapRadio.Change(true);

            Assert.True(
                cut.Find("[data-testid='transfer-mode-swap']")
                    .IsChecked());

            var transferRadio = cut.Find("[data-testid='transfer-mode-transfer']");
            transferRadio.Change(true);

            // Assert
            Assert.True(
                cut.Find("[data-testid='transfer-mode-transfer']")
                    .IsChecked());

            Assert.False(
                cut.Find("[data-testid='transfer-mode-swap']")
                    .IsChecked());
        }

        [Fact]
        public void SelectingDifferentDestinationsUpdates()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC1", "Occupant 1"),
                    new("OCC2", "Occupant 2")
                ]
            );

            var fullDestination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Full Thunderbird",
                thunderbirdCode: "full-thunderbird",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 1,
                occupants:
                [
                    new("OCC3", "Occupant 3")
                ]
            );

            var emptyDestination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Empty Thunderbird",
                thunderbirdCode: "empty-thunderbird",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants: []
            );

            var partiallyOccupiedDestination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Partially Occupied Thunderbird",
                thunderbirdCode: "partially-occupied-thunderbird",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC4", "Occupant 4")
                ]
            );

            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [fullDestination, emptyDestination, partiallyOccupiedDestination])
            );

            // Act
            cut.Find("#transfer-destination").Change("full-thunderbird");

            var transferRadioFirst = cut.Find("[data-testid='transfer-mode-transfer']");
            Assert.True(transferRadioFirst.IsDisabled());

            var swapRadioFirst = cut.Find("[data-testid='transfer-mode-swap']");
            Assert.False(swapRadioFirst.IsDisabled());

            cut.Find("#transfer-destination").Change("partially-occupied-thunderbird");

            var transferRadioSecond = cut.Find("[data-testid='transfer-mode-transfer']");
            Assert.False(transferRadioSecond.IsDisabled());

            var swapRadioSecond = cut.Find("[data-testid='transfer-mode-swap']");
            Assert.False(swapRadioSecond.IsDisabled());

            cut.Find("#transfer-destination").Change("empty-thunderbird");

            // Assert
            var transferRadioThird = cut.Find("[data-testid='transfer-mode-transfer']");
            Assert.False(transferRadioThird.IsDisabled());

            var swapRadioThird = cut.Find("[data-testid='transfer-mode-swap']");
            Assert.True(swapRadioThird.IsDisabled());
        }

        [Fact]
        public void SelectingTransferDoesNotShowReturningCharacterSelector()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC1", "Occupant 1"),
                    new("OCC2", "Occupant 2")
                ]
            );

            var destination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 2",
                thunderbirdCode: "thunderbird-2",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC3", "Occupant 3")
                ]
            );

            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [destination])
            );

            // Act
            cut.Find("[data-testid='transfer-mode-transfer']").Change(true);

            // Assert
            var returningSelector = cut.FindAll("#returning-character");
            Assert.Empty(returningSelector);
        }

        [Fact]
        public void SelectingSwapShowsReturningCharacterSelectorWhenMultipleOccupantsInDestination()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC1", "Occupant 1")
                ]
            );

            var destination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 2",
                thunderbirdCode: "thunderbird-2",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC3", "Occupant 3"),
                    new("OCC2", "Occupant 2")
                ]
            );

            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [destination])
            );

            // Act
            cut.Find("[data-testid='transfer-mode-swap']").Change(true);

            // Assert
            var returningSelector = Assert.Single(
                cut.FindComponents<CharacterSelector>(),
                component =>
                    component.Instance.CharacterId == "returning-character");

            Assert.Equal(2, returningSelector.Instance.Characters.Count);
            Assert.Contains("Occupant 3", cut.Markup);
            Assert.Contains("Occupant 2", cut.Markup);
        }

        [Fact]
        public void SelectingSwapDoesNotShowReturningCharacterSelectorWhenSingleOccupantInDestination()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC1", "Occupant 1")
                ]
            );

            var destination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 2",
                thunderbirdCode: "thunderbird-2",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC3", "Occupant 3")
                ]
            );

            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [destination])
            );

            // Act
            cut.Find("[data-testid='transfer-mode-swap']").Change(true);

            // Assert
            var returningSelector = cut.FindAll("#returning-character");
            Assert.Empty(returningSelector);

            var summary = cut.Find("[data-testid='returning-character-summary']");
            Assert.Contains("Occupant 3", summary.TextContent);
        }

        [Fact]
        public void ReturningToTransferModeHidesReturningCharacterSelector()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC1", "Occupant 1")
                ]
            );

            var destination = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 2",
                thunderbirdCode: "thunderbird-2",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC3", "Occupant 3"),
                    new("OCC2", "Occupant 2")
                ]
            );

            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [destination])
            );

            // Act
            cut.Find("[data-testid='transfer-mode-swap']").Change(true);

            var selector = cut.FindAll("#returning-character");
            Assert.NotEmpty(selector);

            cut.Find("[data-testid='transfer-mode-transfer']").Change(true);

            // Assert
            var returningSelector = cut.FindAll("#returning-character");
            Assert.Empty(returningSelector);
        }

        [Fact]
        public void ChangingDestinationWhileInSwapModeUpdatesReturningCharacterSelector()
        {
            // Arrange
            var source = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 1",
                thunderbirdCode: "thunderbird-1",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC1", "Occupant 1")
                ]
            );

            var destinationOne = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 2",
                thunderbirdCode: "thunderbird-2",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC3", "Occupant 3"),
                    new("OCC2", "Occupant 2")
                ]
            );

            var destinationTwo = GameDashboardViewModelFactory.CreateMachine(
                thunderbirdDisplayName: "Thunderbird 3",
                thunderbirdCode: "thunderbird-3",
                locationDisplayName: "Location 1",
                locationCode: "location-1",
                characterCapacity: 2,
                occupants:
                [
                    new("OCC4", "Occupant 4"),
                    new("OCC5", "Occupant 5")
                ]
            );

            var cut = Render<TransferCharactersDialog>(
                parameters => parameters
                    .Add(p => p.SourceMachine, source)
                    .Add(p => p.DestinationMachines, [destinationOne, destinationTwo])
            );

            // Act
            cut.Find("#transfer-destination").Change("thunderbird-2");

            cut.Find("[data-testid='transfer-mode-swap']").Change(true);

            var selectorOne = cut.FindAll("#returning-character");
            Assert.NotEmpty(selectorOne);
            Assert.Contains("Occupant 3", cut.Markup);
            Assert.Contains("Occupant 2", cut.Markup);

            cut.Find("#transfer-destination").Change("thunderbird-3");

            // Assert
            var selectorTwo = cut.FindAll("#returning-character");
            Assert.NotEmpty(selectorTwo);
            Assert.Contains("Occupant 4", cut.Markup);
            Assert.Contains("Occupant 5", cut.Markup);
        }
    }
}
