using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components
{
    public partial class CharacterSelector
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<CharacterViewModel> Characters { get; set; } = [];

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