using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Services
{
    public record CharacterTransferResult(
        CharacterTransferOutcome Outcome,
        GameDashboardViewModel? UpdatedGameState = null);
}
