using ThunderbirdsBoardGameEngine.GameState.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.CreateGame;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.Services
{
    public sealed class GamePrerequisiteService(IGameClient gameClient)
    {
        public Task<Guid> CreateGameAsync()
        {
            return CreateGameAsync(["gordon", "virgil"]);
        }

        public async Task<Guid> CreateGameAsync(IReadOnlyList<string> turnOrder)
        {
            var dto = new CreateGameRequestDto
            {
                TurnOrder = turnOrder.ToList()
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