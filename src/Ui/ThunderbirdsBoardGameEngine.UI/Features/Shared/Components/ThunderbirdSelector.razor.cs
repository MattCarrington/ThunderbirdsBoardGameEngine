using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.Shared.Components
{
    public partial class ThunderbirdSelector
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<SelectionOptionViewModel> Thunderbirds { get; set; } = [];

        [Parameter]
        public string? SelectedThunderbirdCode { get; set; }

        [Parameter]
        public EventCallback<string> SelectedThunderbirdCodeChanged { get; set; }

        private Task OnChanged(ChangeEventArgs e)
        {
            return SelectedThunderbirdCodeChanged.InvokeAsync(e.Value?.ToString() ?? string.Empty);
        }
    }
}
