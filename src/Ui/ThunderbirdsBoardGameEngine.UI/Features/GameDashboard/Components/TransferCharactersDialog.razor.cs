using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components
{
    public partial class TransferCharactersDialog : IAsyncDisposable
    {
        [Parameter, EditorRequired]
        public ThunderbirdMachinesDashboardViewModel SourceMachine { get; set; } = null!;

        [Parameter, EditorRequired]
        public IReadOnlyList<ThunderbirdMachinesDashboardViewModel> DestinationMachines { get; set; } = [];

        [Parameter]
        public EventCallback Cancelled { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = null!;

        [Parameter]
        public EventCallback<CharacterTransferSelection> TransferConfirmed { get; set; }

        [Parameter]
        public bool IsTransferring { get; set; }

        private IReadOnlyList<SelectionOptionViewModel> CharacterOptions =>
            SourceMachine.Occupants
                .Select(occupant => new SelectionOptionViewModel(
                    occupant.CharacterCode,
                    occupant.CharacterDisplayName))
                .ToArray();

        private IReadOnlyList<SelectionOptionViewModel> DestinationOptions =>
            DestinationMachines
                .Select(t => new SelectionOptionViewModel(
                    t.ThunderbirdCode,
                    t.ThunderbirdDisplayName))
                .ToArray();

        private ThunderbirdMachinesDashboardViewModel? SelectedDestination =>
            DestinationMachines.SingleOrDefault(
                machine =>
                    machine.ThunderbirdCode ==
                    _selectedDestinationCode);

        private bool CanTransfer => SelectedDestination is not null
            && SelectedDestination.Occupants.Count < SelectedDestination.CharacterCapacity;

        private bool CanSwap => SelectedDestination is not null
            && SelectedDestination.Occupants.Count > 0;

        private IReadOnlyList<SelectionOptionViewModel> ReturningCharacterOptions =>
            SelectedDestination?.Occupants
                .Select(occupant => new SelectionOptionViewModel(
                    occupant.CharacterCode,
                    occupant.CharacterDisplayName))
                .ToArray()
            ?? [];

        private bool CannotSubmit => IsTransferring
            || string.IsNullOrWhiteSpace(_selectedCharacterCode)
            || string.IsNullOrWhiteSpace(_selectedDestinationCode)
            || _selectedMode is null
            || (_selectedMode == CharacterTransferMode.Swap
                && string.IsNullOrWhiteSpace(_selectedReturningCharacterCode));

        private ElementReference _dialog;
        private IJSObjectReference? _dialogModule;
        private DotNetObjectReference<TransferCharactersDialog>? _componentReference;
        private string? _selectedCharacterCode;
        private string? _selectedDestinationCode;
        private CharacterTransferMode? _selectedMode;
        private string? _selectedReturningCharacterCode;

        protected override void OnParametersSet()
        {
            if (SourceMachine.Occupants.Count == 1
                && _selectedCharacterCode is null)
            {
                _selectedCharacterCode = SourceMachine.Occupants[0].CharacterCode;
            }

            if (DestinationMachines.Count == 1
                && _selectedDestinationCode is null)
            {
                DestinationChanged(DestinationMachines[0].ThunderbirdCode);
            }
        }

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
            return IsTransferring
                ? Task.CompletedTask
                : CancelAsync();
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

        private void SelectMode(CharacterTransferMode mode)
        {
            _selectedMode = mode;

            _selectedReturningCharacterCode =
                mode == CharacterTransferMode.Swap &&
                SelectedDestination?.Occupants.Count == 1
                    ? SelectedDestination.Occupants[0].CharacterCode
                    : null;
        }

        private void DestinationChanged(string? destinationCode)
        {
            _selectedDestinationCode = destinationCode;
            _selectedReturningCharacterCode = null;

            if (CanTransfer)
            {
                SelectMode(CharacterTransferMode.Transfer);
            }
            else if (CanSwap)
            {
                SelectMode(CharacterTransferMode.Swap);
            }
            else
            {
                _selectedMode = null;
            }
        }

        private Task ConfirmAsync()
        {
            if (CannotSubmit)
            {
                return Task.CompletedTask;
            }

            return TransferConfirmed.InvokeAsync(
                new CharacterTransferSelection(
                    SourceMachine.ThunderbirdCode,
                    _selectedCharacterCode!,
                    _selectedDestinationCode!,
                    _selectedMode == CharacterTransferMode.Swap
                        ? _selectedReturningCharacterCode
                        : null));
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
