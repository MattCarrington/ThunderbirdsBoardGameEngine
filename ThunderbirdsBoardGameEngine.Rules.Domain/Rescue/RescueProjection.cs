namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public sealed record RescueProjection(int DifficultyNumber, IReadOnlyList<RescueBonus> Bonuses);
}