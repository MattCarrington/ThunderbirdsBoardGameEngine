using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.Home.Components
{
    public partial class CreateGameDialog : IAsyncDisposable
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<SelectionOptionViewModel> Characters { get; set; } = [];

        [Parameter]
        public bool IsCreating { get; set; }

        [Parameter]
        public string? ErrorMessage { get; set; }

        [Parameter]
        public EventCallback<IReadOnlyList<string>> Confirmed { get; set; }

        [Parameter]
        public EventCallback Cancelled { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = null!;

        [JSInvokable]
        public Task HandleDialogCancelled()
        {
            return CancelAsync();
        }

        private ElementReference _dialog;
        private IJSObjectReference? _dialogModule;
        private DotNetObjectReference<CreateGameDialog>? _componentReference;

        private readonly List<string?> _selectedCharacters = [null, null];

        private bool CannotSubmit => IsCreating
            || _selectedCharacters.Count is < 2 or > 4
            || _selectedCharacters.Any(string.IsNullOrWhiteSpace)
            || _selectedCharacters.Distinct(StringComparer.Ordinal).Count() != _selectedCharacters.Count;

        private bool CanAddPlayer => !IsCreating
            && _selectedCharacters.Count < 4;

        private bool CanRemovePlayer => !IsCreating
            && _selectedCharacters.Count > 2;

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

            return Confirmed.InvokeAsync(
                _selectedCharacters
                    .Select(character => character!)
                    .ToArray());
        }

        private void AddPlayer()
        {
            if (CanAddPlayer)
            {
                _selectedCharacters.Add(null);
            }
        }

        private void RemovePlayer()
        {
            if (CanRemovePlayer)
            {
                _selectedCharacters.RemoveAt(
                    _selectedCharacters.Count - 1);
            }
        }

        private void SelectCharacter(int index, string? characterCode)
        {
            _selectedCharacters[index] = characterCode;
        }

        private IReadOnlyList<SelectionOptionViewModel> GetOptionsFor(int index)
        {
            var selectedElsewhere = _selectedCharacters
                .Where((_, selectedIndex) => selectedIndex != index)
                .Where(character => !string.IsNullOrWhiteSpace(character))
                .ToHashSet(StringComparer.Ordinal);

            return Characters
                .Where(character => !selectedElsewhere.Contains(character.Key))
                .ToArray();
        }

        private async Task CancelAsync()
        {
            if (IsCreating)
            {
                return;
            }

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
