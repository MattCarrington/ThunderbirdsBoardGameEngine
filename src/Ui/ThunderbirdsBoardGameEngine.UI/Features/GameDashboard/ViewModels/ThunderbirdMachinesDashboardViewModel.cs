namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels
{
    public record ThunderbirdMachinesDashboardViewModel(
        string ThunderbirdCode,
        string ThunderbirdDisplayName,
        string LocationCode,
        string LocationDisplayName,
        IReadOnlyList<OccupantsDashboardViewModel> Occupants);
}
