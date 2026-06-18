namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels
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
