namespace ThunderbirdsBoardGameEngine.UI.ViewModels
{
    public sealed record DisasterCardViewModel(
        string Code,
        string DisplayName,
        int DifficultyNumber,
        string RescueType,
        string Location,
        IReadOnlyList<BonusConditionViewModel> BonusConditions,
        IReadOnlyList<RewardViewModel> Rewards
    );
}
