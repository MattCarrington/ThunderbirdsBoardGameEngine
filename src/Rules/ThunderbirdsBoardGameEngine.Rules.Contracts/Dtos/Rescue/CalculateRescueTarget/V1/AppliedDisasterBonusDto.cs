namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1
{
    /// <summary>
    /// Represents a data transfer object that describes a bonus applied as a result of a disaster resolution calculation.
    /// </summary>
    public record AppliedDisasterBonusDto
    {
        /// <summary>
        /// Gets the unique key associated with the bonus.
        /// </summary>
        public required string BonusKey { get; init; }

        /// <summary>
        /// Gets the bonus value associated with this instance.
        /// </summary>
        public required int BonusValue { get; init; }

        /// <summary>
        /// Gets the type of the source that produced the bonus.
        /// </summary>
        public required string SourceType { get; init; }
    }
}
