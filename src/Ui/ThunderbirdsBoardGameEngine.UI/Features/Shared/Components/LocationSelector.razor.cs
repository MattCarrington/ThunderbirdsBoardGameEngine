using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.Shared.Components
{
    public partial class LocationSelector
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<SelectionOptionViewModel> Locations { get; set; } = [];

        [Parameter]
        public string? SelectedLocationKey { get; set; }

        [Parameter]
        public EventCallback<string?> SelectedLocationKeyChanged { get; set; }

        [Parameter]
        public string LocationId { get; set; } = Guid.NewGuid().ToString();

        [Parameter]
        public string LocationTitle { get; set; } = string.Empty;

        private Task OnChanged(ChangeEventArgs e)
        {
            var value = e.Value?.ToString();

            return SelectedLocationKeyChanged.InvokeAsync(
                string.IsNullOrWhiteSpace(value)
                    ? null
                    : value);
        }
    }
}
