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
        public int CardId { get; }

        /// <summary>
        /// Initializes a new instance of the DisasterCardNotFoundException class for the specified disaster card
        /// identifier.
        /// </summary>
        /// <param name="cardId">The unique identifier of the disaster card that could not be found.</param>
        public DisasterCardNotFoundException(int cardId)
            : base($"Disaster card with ID {cardId} was not found.")
        {
            CardId = cardId;
        }
    }
}
