using Bunit;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures;
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
    }
}
