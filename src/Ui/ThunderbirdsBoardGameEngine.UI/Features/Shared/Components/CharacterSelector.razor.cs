using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.Shared.Components
{
    public partial class CharacterSelector
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<SelectionOptionViewModel> Characters { get; set; } = [];

        [Parameter]
        public string? SelectedCharacterCode { get; set; }

        [Parameter]
        public EventCallback<string?> SelectedCharacterCodeChanged { get; set; }

        private Task OnChanged(ChangeEventArgs e)
        {
            return SelectedCharacterCodeChanged.InvokeAsync(e.Value?.ToString());
        }
    }
}