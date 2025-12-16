namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record CalculateRescueTargetRequest(int DisasterCardId, IReadOnlyCollection<string> AppliedBonusKeys);
}