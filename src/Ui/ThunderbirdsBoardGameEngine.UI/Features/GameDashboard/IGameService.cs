namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard
{
    public interface IGameService
    {
        Task<GameSessionViewModel?> CreateGameSessionViewModel(CancellationToken cancellationToken = default);
        Task<GameSessionViewModel?> GetGameSessionViewModel(Guid gameId, CancellationToken cancellationToken = default);
        Task<GameSessionViewModel?> MoveThunderbirdAsync(Guid gameId, string thunderbirdCode, string destinationLocationCode, CancellationToken cancellationToken = default);
    }
}
