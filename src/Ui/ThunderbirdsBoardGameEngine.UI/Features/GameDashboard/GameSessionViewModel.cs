namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard
{
    public record GameSessionViewModel(Guid GameId, List<ThunderbirdLocationViewModel> ThunderbirdLocations);
}
