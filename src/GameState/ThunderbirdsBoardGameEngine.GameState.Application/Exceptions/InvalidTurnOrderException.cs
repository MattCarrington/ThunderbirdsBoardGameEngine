namespace ThunderbirdsBoardGameEngine.GameState.Application.Exceptions
{
    [Serializable]
    internal class InvalidTurnOrderException : Exception
    {
        public InvalidTurnOrderException()
        {
        }

        public InvalidTurnOrderException(string? message) : base(message)
        {
        }

        public InvalidTurnOrderException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}