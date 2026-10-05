namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels
{
    public record GameDashboardViewModel(
        Guid GameId,
        IReadOnlyList<ThunderbirdMachinesDashboardViewModel> ThunderbirdMachines);
}
