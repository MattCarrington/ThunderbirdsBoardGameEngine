using ThunderbirdsBoardGameEngine.GameState.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.CreateGame;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.Services
{
    public sealed class GamePrerequisiteService(IGameClient gameClient)
    {
        public async Task<Guid> CreateGameAsync()
        {
            var dto = new CreateGameRequestDto
            {
                TurnOrder =
                [
                    "gordon",
                    "virgil"
                ]
            };

            var result = await gameClient.CreateGameAsync(dto);

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