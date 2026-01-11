namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1
{
    /// <summary>
    /// Represents the result of calculating a rescue target, including the required target number, total bonus, and
    /// details of applied disaster bonuses.
    /// </summary>
    public record CalculateRescueTargetResponseDto
    {
        /// <summary>
        /// Gets the calculated target number for the rescue operation.
        /// </summary>
        public required int TargetNumber { get; init; }

        /// <summary>
        /// Gets the total bonus amount associated with this instance.
        /// </summary>
        public required int TotalBonus { get; init; }

        /// <summary>
        /// Gets the collection of disaster bonuses that have been applied.
        /// </summary>
        public required IReadOnlyCollection<AppliedDisasterBonusDto> AppliedDisasterBonuses { get; init; }
    }
}
