namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public sealed record RescueContext(int DifficultyNumber, IReadOnlyList<RescueContextBonus> Bonuses);
}