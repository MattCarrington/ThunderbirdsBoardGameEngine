namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1
{
    public record CalculateRescueTargetResponseDto
    {
        public required int TargetNumber { get; init; }

        public required int TotalBonus { get; init; }

        public required IReadOnlyCollection<AppliedBonusDto> AppliedBonuses { get; init; }
    }
}
