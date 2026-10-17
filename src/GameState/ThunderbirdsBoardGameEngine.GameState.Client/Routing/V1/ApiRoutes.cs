namespace ThunderbirdsBoardGameEngine.GameState.Client.Routing.V1
{
    internal static class ApiRoutes
    {
        public const string Games = "api/games";

        public static string GetGameState(Guid gameId)
        {
            return $"api/games/{gameId}";
        }

        public static string MoveThunderbirdMachine(Guid gameId, string thunderbirdCode)
        {
            return $"api/games/{gameId}/thunderbird-machines/{Uri.EscapeDataString(thunderbirdCode)}/movements";
        }
    }
}
