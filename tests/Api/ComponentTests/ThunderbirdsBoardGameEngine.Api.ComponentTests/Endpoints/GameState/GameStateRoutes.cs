namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.GameState
{
    public class GameStateRoutes
    {
        private const string Games = "/api/games";

        public static string CreateGame()
        {
            return Games;
        }

        public static string GetGame(Guid gameId)
        {
            return $"{Games}/{gameId}";
        }

        public static string MoveThunderbird(Guid gameId, string thunderbirdCode)
        {
            return $"{Games}/{gameId}/thunderbird-machines/{thunderbirdCode}/move";
        }
    }
}
