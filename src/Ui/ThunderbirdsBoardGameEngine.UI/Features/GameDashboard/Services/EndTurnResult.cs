using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Services
{
    public sealed record EndTurnResult(EndTurnOutcome Outcome, GameDashboardViewModel? UpdatedGameState = null);
}
