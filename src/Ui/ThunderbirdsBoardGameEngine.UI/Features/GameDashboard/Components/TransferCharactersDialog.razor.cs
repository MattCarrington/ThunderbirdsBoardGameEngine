using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components
{
    public partial class TransferCharactersDialog
    {
        [Parameter, EditorRequired]
        public ThunderbirdMachinesDashboardViewModel SourceMachine { get; set; } = null!;

        [Parameter, EditorRequired]
        public IReadOnlyList<ThunderbirdMachinesDashboardViewModel>
            DestinationMachines
        { get; set; } = [];

        [Parameter]
        public EventCallback Cancelled { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = null!;

        private ElementReference _dialog;
        private IJSObjectReference? _dialogModule;

        private DotNetObjectReference<TransferCharactersDialog>?
            _componentReference;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            _dialogModule =
                await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./js/dialog.js");

            _componentReference =
                DotNetObjectReference.Create(this);

            await _dialogModule.InvokeVoidAsync(
                "show",
                _dialog,
                _componentReference);
        }

        [JSInvokable]
        public Task HandleDialogCancelled()
        {
            return CancelAsync();
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

        public async ValueTask DisposeAsync()
        {
            _componentReference?.Dispose();

            if (_dialogModule is not null)
            {
                await _dialogModule.DisposeAsync();
            }
        }
    }
}
