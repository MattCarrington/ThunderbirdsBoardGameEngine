namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels
{
    public record TurnSummaryViewModel(
        IReadOnlyList<TurnOrderCharacterDisplayModel> TurnOrder,
        TurnOrderCharacterDisplayModel ActiveCharacter);
}
