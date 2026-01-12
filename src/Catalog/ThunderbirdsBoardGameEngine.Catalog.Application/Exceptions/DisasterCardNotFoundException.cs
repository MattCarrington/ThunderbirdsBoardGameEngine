namespace ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a disaster card with the specified identifier cannot be found.
    /// </summary>
    public sealed class DisasterCardNotFoundException : NotFoundException
    {
        /// <summary>
        /// Gets the unique identifier for the card.
        /// </summary>
        public string CardCode { get; }

        /// <summary>
        /// Initializes a new instance of the DisasterCardNotFoundException class for the specified disaster card
        /// identifier.
        /// </summary>
        /// <param name="cardCode">The unique identifier of the disaster card that could not be found.</param>
        public DisasterCardNotFoundException(string cardCode)
            : base($"Disaster card with Code {cardCode} was not found.", "DisasterCard", cardCode)
        {
            CardCode = cardCode;
        }
    }
}
