using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Components
{
    public partial class ThunderbirdSelector
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<ThunderbirdMovementOptions> Thunderbirds { get; set; } = [];

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
