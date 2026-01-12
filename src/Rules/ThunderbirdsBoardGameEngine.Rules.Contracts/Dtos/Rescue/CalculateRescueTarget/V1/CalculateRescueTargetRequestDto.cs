namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1
{
    /// <summary>
    /// Represents the data required to calculate a rescue target, including the set of active disaster bonus keys.
    /// </summary>
    public record CalculateRescueTargetRequestDto
    {
        /// <summary>
        /// Gets the collection of bonus keys that are currently active for disasters.
        /// </summary>
        public required IReadOnlyCollection<string> PresentDisasterBonusKeys { get; init; }
    }
}
