namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs
{
    public sealed record DisasterInput(
        string Name,
        int DifficultyNumber,
        string Location,
        string RescueType,
        IReadOnlyList<BonusInput> Bonuses,
        IReadOnlyList<string> Rewards);
}
