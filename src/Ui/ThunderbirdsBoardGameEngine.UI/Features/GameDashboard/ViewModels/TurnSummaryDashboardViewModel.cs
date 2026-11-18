namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels
{
    public record TurnSummaryDashboardViewModel(
        IReadOnlyList<TurnOrderCharacterDashboardViewModel> TurnOrder,
        TurnOrderCharacterDashboardViewModel ActiveCharacter);
}
