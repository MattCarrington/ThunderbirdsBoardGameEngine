using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Services
{
    public sealed record ThunderbirdMovementResult(ThunderbirdMovementOutcome Outcome, GameDashboardViewModel? UpdatedGameState = null);
}
