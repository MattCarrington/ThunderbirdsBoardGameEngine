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

        private bool _isEndingTurn;

        private async Task EndTurnAsync()
        {
            if (_isEndingTurn)
            {
                return;
            }

            _isEndingTurn = true;

            try
            {
                await EndTurnRequested.InvokeAsync();
            }
            finally
            {
                _isEndingTurn = false;
            }
        }
    }
}
