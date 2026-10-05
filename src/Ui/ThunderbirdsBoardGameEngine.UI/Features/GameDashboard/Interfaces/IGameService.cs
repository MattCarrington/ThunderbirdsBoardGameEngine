using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Interfaces
{
    public interface IGameService
    {
        Task<Guid> CreateGameAsync();

        Task<GameDashboardViewModel?> GetGameAsync(Guid gameId);
    }
}
