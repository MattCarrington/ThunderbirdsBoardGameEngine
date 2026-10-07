using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.Shared.Components
{
    public partial class LocationSelector
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<LocationOptionsViewModel> Locations { get; set; } = [];

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
