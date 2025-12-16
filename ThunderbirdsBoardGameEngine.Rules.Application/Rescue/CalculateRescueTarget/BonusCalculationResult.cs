
namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record BonusCalculationResult
    {
        public required int TargetRoll { get; init; }

        public required int TotalBonus { get; init; }

        public IReadOnlyList<RescueContextBonus> AppliedBonuses { get; init; } = Array.Empty<RescueContextBonus>();
    }
}
