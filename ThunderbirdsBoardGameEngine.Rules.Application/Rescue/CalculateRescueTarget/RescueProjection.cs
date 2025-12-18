namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public sealed record RescueProjection(int DifficultyNumber, IReadOnlyList<RescueBonus> Bonuses);
}