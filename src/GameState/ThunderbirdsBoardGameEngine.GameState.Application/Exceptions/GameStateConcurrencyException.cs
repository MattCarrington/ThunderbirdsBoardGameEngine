namespace ThunderbirdsBoardGameEngine.GameState.Application.Exceptions
{
    public sealed class GameStateConcurrencyException : Exception
    {
        public GameStateConcurrencyException()
        : base("The game state was modified by another operation.")
        {
        }

        public GameStateConcurrencyException(Exception innerException)
            : base("The game state was modified by another operation.", innerException)
        {
        }
    }
}
