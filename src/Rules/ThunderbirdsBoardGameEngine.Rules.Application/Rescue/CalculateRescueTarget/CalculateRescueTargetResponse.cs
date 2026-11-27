using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Modifiers;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record CalculateRescueTargetResponse(int TargetNumber, int TotalBonus, IReadOnlyCollection<AppliedRescueModifier> AppliedBonuses);
}