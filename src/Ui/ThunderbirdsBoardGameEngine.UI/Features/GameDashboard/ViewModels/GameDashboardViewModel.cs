namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels
{
    public record GameDashboardViewModel(
        Guid GameId,
        IReadOnlyList<ThunderbirdMachinesDashboardViewModel> ThunderbirdMachines,
        IReadOnlyList<TurnOrderCharacterDisplayModel> TurnOrder,
        string ActiveCharacterDisplayName,
        string ActiveCharacterCode);
}
