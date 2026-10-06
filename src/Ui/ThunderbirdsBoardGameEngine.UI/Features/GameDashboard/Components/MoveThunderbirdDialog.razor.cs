using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components
{
    public partial class MoveThunderbirdDialog
    {
        [Parameter, EditorRequired]
        public ThunderbirdMachinesDashboardViewModel Machine
        {
            get;
            set;
        } = null!;

        [Parameter, EditorRequired]
        public IReadOnlyList<MovementLocationOptions> Destinations
        {
            get;
            set;
        } = [];

        [Parameter]
        public bool IsLoadingDestinations { get; set; }

        [Parameter]
        public bool IsMoving { get; set; }

        [Parameter]
        public string? ErrorMessage { get; set; }

        [Parameter]
        public EventCallback<string> MoveConfirmed { get; set; }

        [Parameter]
        public EventCallback Cancelled { get; set; }

        private string _selectedDestination = string.Empty;

        private bool CannotSubmit =>
            IsLoadingDestinations ||
            IsMoving ||
            string.IsNullOrWhiteSpace(_selectedDestination);

        private Task ConfirmAsync()
        {
            if (CannotSubmit)
            {
                return Task.CompletedTask;
            }

            return MoveConfirmed.InvokeAsync(_selectedDestination);
        }

        private Task CancelAsync()
        {
            return Cancelled.InvokeAsync();
        }
    }
}
