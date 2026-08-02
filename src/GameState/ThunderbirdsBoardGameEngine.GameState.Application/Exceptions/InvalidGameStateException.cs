namespace ThunderbirdsBoardGameEngine.GameState.Application.Exceptions
{
    public class InvalidGameStateException : Exception
    {
        public InvalidGameStateException(string message)
            : base(message)
        {
        }
    }
}
