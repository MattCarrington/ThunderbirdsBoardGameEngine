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

        public static TurnOrderCharacterDashboardViewModel CreateTurnOrderCharacter(
            string characterCode = "scott",
            string characterDisplayName = "Scott")
        {
            return new TurnOrderCharacterDashboardViewModel(
                CharacterCode: characterCode,
                CharacterDisplayName: characterDisplayName);
        }

        public static TurnSummaryDashboardViewModel CreateTurnSummary(
            IReadOnlyList<TurnOrderCharacterDashboardViewModel>? turnOrder = null,
            string activeCharacter = "scott",
            string activeCharacterDisplayName = "Scott")
        {
            return new TurnSummaryDashboardViewModel(
                TurnOrder: turnOrder
                    ?? [CreateTurnOrderCharacter(), CreateTurnOrderCharacter("virgil", "Virgil")],
                ActiveCharacter: CreateTurnOrderCharacter(activeCharacter, activeCharacterDisplayName));
        }

        public static GameDashboardViewModel CreateDashboard(
            Guid? gameId = null,
            IReadOnlyList<ThunderbirdMachinesDashboardViewModel>? machines = null,
            TurnSummaryDashboardViewModel? turnSummary = null)
        {
            return new GameDashboardViewModel(
                GameId: gameId
                    ?? Guid.NewGuid(),
                TurnSummary: turnSummary
                    ?? CreateTurnSummary(),
                ThunderbirdMachines: machines
                    ?? [CreateMachine()]);
        }
    }
}