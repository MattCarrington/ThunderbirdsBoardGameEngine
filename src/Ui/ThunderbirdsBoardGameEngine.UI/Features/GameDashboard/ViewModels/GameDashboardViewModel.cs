namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels
{
    public record GameDashboardViewModel(
        Guid GameId,
        IReadOnlyList<ThunderbirdMachinesDashboardViewModel> ThunderbirdMachines,
        IReadOnlyList<TurnOrderDisplayModel> TurnOrder,
        string ActiveCharacterDisplayName,
        string ActiveCharacterCode);
}
