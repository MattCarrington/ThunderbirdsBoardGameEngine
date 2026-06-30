using Microsoft.AspNetCore.Components;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard
{
    public partial class GameDashboardPage
    {
        [Inject]
        public IGameService GameService { get; set; } = null!;

        private GameSessionViewModel _gameSessionViewModel { get; set; } = null!;

        private async Task CreateGame()
        {
            _gameSessionViewModel = await GameService.GetGameSessionViewModel() ?? throw new InvalidOperationException("Failed to create game session.");
        }
    }
}
