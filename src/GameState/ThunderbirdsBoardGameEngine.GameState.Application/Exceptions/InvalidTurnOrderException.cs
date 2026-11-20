namespace ThunderbirdsBoardGameEngine.GameState.Application.Exceptions
{
    public sealed class InvalidTurnOrderException : Exception
    {
        private InvalidTurnOrderException(string? message) : base(message)
        {
        }

        public static InvalidTurnOrderException TooFewCharacters(int actualCount)
        {
            return new($"A game requires at least two player characters, but received {actualCount}.");
        }

        public static InvalidTurnOrderException TooManyCharacters(int actualCount)
        {
            return new($"A game supports a maximum of four player characters, but received {actualCount}.");
        }

        public static InvalidTurnOrderException DuplicateCharacters()
        {
            return new("A character can appear in the turn order only once.");
        }
    }
}