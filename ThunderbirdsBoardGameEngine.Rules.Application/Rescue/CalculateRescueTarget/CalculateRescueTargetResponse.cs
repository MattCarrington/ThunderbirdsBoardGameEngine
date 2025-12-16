namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record CalculateRescueTargetResponse(int TargetNumber, int TotalBonus, IReadOnlyList<RescueContextBonus> AppliedBonuses);
}