using ThunderbirdsBoardGameEngine.GameState.Client.Interfaces.V1;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.Services
{
    public sealed class GamePrerequisiteService(IGameClient gameClient)
    {
        public async Task<Guid> CreateGameAsync()
        {
            var result = await gameClient.CreateGameAsync();

            if (!result.Success || result.Data is null)
            {
                throw new InvalidOperationException(
                    $"Failed to create prerequisite game. " +
                    $"Status: {result.StatusCode}.");
            }

            return result.Data.GameId;
        }
    }
}