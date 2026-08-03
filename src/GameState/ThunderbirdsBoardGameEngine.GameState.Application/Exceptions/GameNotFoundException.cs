namespace ThunderbirdsBoardGameEngine.GameState.Application.Exceptions
{
    public sealed class GameNotFoundException : Exception
    {
        public GameNotFoundException()
            : base("Game not found.")
        {
        }
    }
}
