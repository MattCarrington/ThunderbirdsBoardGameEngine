using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components
{
    public partial class MoveThunderbirdDialog : IAsyncDisposable
    {
        [Parameter, EditorRequired]
        public ThunderbirdMachinesDashboardViewModel Machine { get; set; } = null!;

        [Parameter, EditorRequired]
        public IReadOnlyList<LocationOptionsViewModel> Destinations { get; set; } = [];

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

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = null!;

        [JSInvokable]
        public Task HandleDialogCancelled()
        {
            return Cancelled.InvokeAsync();
        }

        private ElementReference _dialog;
        private IJSObjectReference? _dialogModule;
        private DotNetObjectReference<MoveThunderbirdDialog>? _componentReference;

        private string _selectedDestination = string.Empty;

        private bool CannotSubmit =>
            IsLoadingDestinations ||
            IsMoving ||
            string.IsNullOrWhiteSpace(_selectedDestination);

        public async ValueTask DisposeAsync()
        {
            _componentReference?.Dispose();

            if (_dialogModule is not null)
            {
                await _dialogModule.DisposeAsync();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            _dialogModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/dialog.js");

            _componentReference = DotNetObjectReference.Create(this);

            await _dialogModule.InvokeVoidAsync(
                "show",
                _dialog,
                _componentReference);
        }

        private Task ConfirmAsync()
        {
            if (CannotSubmit)
            {
                return Task.CompletedTask;
            }

            return MoveConfirmed.InvokeAsync(_selectedDestination);
        }

        private async Task CancelAsync()
        {
            if (_dialogModule is not null)
            {
                await _dialogModule.InvokeVoidAsync(
                    "close",
                    _dialog);
            }

            await Cancelled.InvokeAsync();
        }
    }
}
