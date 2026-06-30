using ThunderbirdsBoardGameEngine.GameState.Client.Clients;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard
{
    public class GameService : IGameService
    {
        private IGameClient _gameClient;

        public GameService(IGameClient gameClient)
        {
            _gameClient = gameClient;
        }

        public async Task<GameSessionViewModel?> GetGameSessionViewModel(CancellationToken cancellationToken = default)
        {
            var result = await _gameClient.CreateGameStateAsync(cancellationToken);

            if (result.Success)
            {
                var gameSession = result.Data ?? throw new InvalidOperationException("Game session data is null.");
                var thunderbirdLocations = gameSession.ThunderbirdLocation.Select(tl => new ThunderbirdLocationViewModel(
                    tl.ThunderbirdCode,
                    tl.ThunderbirdCode,
                    tl.LocationCode,
                    tl.LocationCode
                )).ToList();

                return new GameSessionViewModel(gameSession.GameId, thunderbirdLocations);
            }

            // Handle error case as needed
            return null;
        }
    }
}
