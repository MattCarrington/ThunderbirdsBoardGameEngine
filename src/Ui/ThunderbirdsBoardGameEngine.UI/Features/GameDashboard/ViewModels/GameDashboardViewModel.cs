namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels
{
    public record GameDashboardViewModel(
        Guid GameId,
        TurnSummaryViewModel TurnSummary,
        IReadOnlyList<ThunderbirdMachinesDashboardViewModel> ThunderbirdMachines);
}
