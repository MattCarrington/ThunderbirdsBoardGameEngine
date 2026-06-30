namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard
{
    public interface IGameService
    {
        Task<GameSessionViewModel?> GetGameSessionViewModel(CancellationToken cancellationToken = default);
    }
}
