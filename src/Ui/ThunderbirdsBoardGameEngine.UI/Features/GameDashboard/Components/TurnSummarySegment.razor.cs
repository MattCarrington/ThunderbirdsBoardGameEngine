using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components
{
    public partial class TurnSummarySegment
    {
        [Parameter, EditorRequired]
        public TurnSummaryDashboardViewModel Summary { get; set; } = null!;

        [Parameter]
        public EventCallback EndTurnRequested { get; set; }

        [Parameter]
        public bool IsEndingTurn { get; set; }

        [Parameter]
        public string? ErrorMessage { get; set; }

        [Parameter]
        public bool IsMutationPending { get; set; }
    }
}
