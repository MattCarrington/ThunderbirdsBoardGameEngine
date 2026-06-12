namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1
{
    /// <summary>
    /// Represents the data required to validate a movement action from a starting location to a destination location.
    /// </summary>
    public sealed record ValidateMovementRequestDto
    {
        /// <summary>
        /// Gets the unique key identifying the starting location of the movement action.
        /// </summary>
        public required string StartLocation { get; init; }

        /// <summary>
        /// Gets the unique key identifying the destination location of the movement action.
        /// </summary>
        public required string DestinationLocation { get; init; }
    }
}
