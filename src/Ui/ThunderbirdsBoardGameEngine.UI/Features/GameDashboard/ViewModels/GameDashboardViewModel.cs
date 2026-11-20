namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels
{
    public record GameDashboardViewModel(
        Guid GameId,
        TurnSummaryDashboardViewModel TurnSummary,
        IReadOnlyList<ThunderbirdMachinesDashboardViewModel> ThunderbirdMachines);
}
