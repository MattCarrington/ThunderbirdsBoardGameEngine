namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public sealed record DisasterContribution(int DifficultyNumber, IReadOnlyList<DisasterBonus> Bonuses);
}