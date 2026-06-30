using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard
{
    public partial class GameDashboardPage
    {
        [Inject]
        public IGameService GameService { get; set; } = null!;

        [Inject]
        public IMovementClientService MovementClientService { get; set; } = null!;

        private GameSessionViewModel _gameSessionViewModel { get; set; } = null!;

        private ThunderbirdLocationViewModel? _selectedThunderbird;

        private string? _selectedDestination;

        private IReadOnlyList<MovementLocationOptions> _locations = Array.Empty<MovementLocationOptions>();

        private async Task CreateGame()
        {
            _gameSessionViewModel = await GameService.CreateGameSessionViewModel() ?? throw new InvalidOperationException("Failed to create game session.");
        }

        private async Task ShowMoveDialog(ThunderbirdLocationViewModel thunderbird)
        {
            _locations = await MovementClientService.GetAccessibleLocationsAsync(thunderbird.ThunderbirdCode);
            _selectedThunderbird = thunderbird;
        }

        private async Task MoveThunderbird()
        {
            if (_selectedThunderbird is null || string.IsNullOrEmpty(_selectedDestination))
            {
                return;
            }

            _gameSessionViewModel = await GameService.MoveThunderbirdAsync(_gameSessionViewModel.GameId, _selectedThunderbird.ThunderbirdCode, _selectedDestination)
                ?? throw new InvalidOperationException("Failed to retrieve game session after moving Thunderbird.");

            CloseDialog();
        }

        private void CloseDialog()
        {
            _selectedDestination = null;
            _selectedThunderbird = null!;
        }
    }
}
