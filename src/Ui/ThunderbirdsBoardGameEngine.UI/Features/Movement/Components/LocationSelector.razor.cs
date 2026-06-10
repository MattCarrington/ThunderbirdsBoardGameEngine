using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Components
{
    public partial class LocationSelector
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<MovementLocationOptions> Locations { get; set; } = [];

        [Parameter]
        public string? SelectedLocationKey { get; set; }

        [Parameter]
        public EventCallback<string> SelectedLocationKeyChanged { get; set; }

        [Parameter]
        public string LocationId { get; set; } = Guid.NewGuid().ToString();

        [Parameter]
        public string LocationTitle { get; set; } = string.Empty;

        private Task OnChanged(ChangeEventArgs e)
        {
            return SelectedLocationKeyChanged.InvokeAsync(e.Value?.ToString() ?? string.Empty);
        }
    }
}
