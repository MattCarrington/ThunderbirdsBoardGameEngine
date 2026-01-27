namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1
{
    /// <summary>
    /// Represents the data required to calculate a rescue target
    /// for a specific disaster and performing character, including the set of active disaster bonus keys.
    /// </summary>
    public record CalculateRescueTargetRequestDto
    {
        /// <summary>
        /// Gets the collection of bonus keys that are currently active for disasters.
        /// </summary>
        public required IReadOnlyCollection<string> PresentDisasterBonusKeys { get; init; }

        /// <summary>
        /// Gets the unique key identifying the character performing the rescue action.
        /// </summary>
        /// /// <remarks>
        /// This property was added to V1 to enforce a core domain invariant.
        /// While technically a breaking change, the contract is still evolving
        /// and has not yet been declared stable.
        /// </remarks>
        public required string PerformingCharacterKey { get; init; }
    }
}
