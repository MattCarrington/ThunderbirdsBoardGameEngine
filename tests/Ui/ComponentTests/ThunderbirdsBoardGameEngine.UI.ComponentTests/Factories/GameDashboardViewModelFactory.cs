using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Factories
{
    public static class GameDashboardViewModelFactory
    {
        public static ThunderbirdMachinesDashboardViewModel CreateMachine(
            string thunderbirdCode = "thunderbird-1",
            string thunderbirdDisplayName = "Thunderbird 1",
            string locationCode = "south-pacific",
            string locationDisplayName = "South Pacific",
            IReadOnlyList<OccupantsDashboardViewModel>? occupants = null,
            int characterCapacity = 1)
        {
            return new ThunderbirdMachinesDashboardViewModel(
                ThunderbirdCode: thunderbirdCode,
                ThunderbirdDisplayName: thunderbirdDisplayName,
                LocationCode: locationCode,
                LocationDisplayName: locationDisplayName,
                Occupants: occupants ?? [],
                CharacterCapacity: characterCapacity);
        }

        public static OccupantsDashboardViewModel CreateOccupant(
            string characterCode = "scott",
            string characterDisplayName = "Scott")
        {
            return new OccupantsDashboardViewModel(
                CharacterCode: characterCode,
                CharacterDisplayName: characterDisplayName);
        }

        public static TurnOrderCharacterDisplayModel CreateTurnOrderCharacter(
            string characterCode = "scott",
            string characterDisplayName = "Scott")
        {
            return new TurnOrderCharacterDisplayModel(
                CharacterCode: characterCode,
                CharacterDisplayName: characterDisplayName);
        }

        public static TurnSummaryViewModel CreateTurnSummary(
            IReadOnlyList<TurnOrderCharacterDisplayModel>? turnOrder = null,
            string activeCharacter = "scott")
        {
            return new TurnSummaryViewModel(
                TurnOrder: turnOrder
                    ?? [CreateTurnOrderCharacter(), CreateTurnOrderCharacter("virgil", "Virgil")],
                ActiveCharacter: CreateTurnOrderCharacter(activeCharacter));
        }

        public static GameDashboardViewModel CreateDashboard(
            Guid? gameId = null,
            IReadOnlyList<ThunderbirdMachinesDashboardViewModel>? machines = null)
        {
            return new GameDashboardViewModel(
                GameId: gameId
                    ?? Guid.NewGuid(),
                TurnSummary: CreateTurnSummary(),
                ThunderbirdMachines: machines
                    ?? [CreateMachine()]);
        }
    }
}