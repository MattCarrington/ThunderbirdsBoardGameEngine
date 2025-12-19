using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record CalculateRescueTargetResponse(int TargetNumber, int TotalBonus, IReadOnlyList<DisasterBonus> AppliedBonuses);
}