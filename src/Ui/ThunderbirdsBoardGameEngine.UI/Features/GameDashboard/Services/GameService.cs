using System.Net;
using ThunderbirdsBoardGameEngine.GameState.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Mappers;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Services
{
    public class GameService : IGameService
    {
        private readonly IGameClient _client;
        private readonly GameDashboardMapper _mapper;
        public GameService(IGameClient client, GameDashboardMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<Guid> CreateGameAsync()
        {
            var result = await _client.CreateGameAsync();

            if (!result.Success || result.Data is null)
            {
                throw new InvalidOperationException("The game could not be created.");
            }

            return result.Data.GameId;
        }

        public async Task<GameDashboardViewModel?> GetGameAsync(Guid gameId)
        {
            var result = await _client.GetGameStateAsync(gameId);

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!result.Success || result.Data is null)
            {
                throw new InvalidOperationException("The game could not be retrieved.");
            }

            return _mapper.ToViewModel(result.Data);
        }
    }
}
