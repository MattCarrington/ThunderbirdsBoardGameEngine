using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components
{
    public partial class TurnSummarySegment
    {
        [Parameter, EditorRequired]
        public TurnSummaryDashboardViewModel Summary { get; set; } = null!;
    }
}
