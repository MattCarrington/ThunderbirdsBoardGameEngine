using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components
{
    public partial class DisasterCardSelector
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<DisasterCardSummaryViewModel> DisasterCards { get; set; } = [];

        [Parameter]
        public string? SelectedDisasterCardCode { get; set; }

        [Parameter]
        public EventCallback<string?> SelectedDisasterCardCodeChanged { get; set; }

        private Task OnChanged(ChangeEventArgs e)
        {
            var value = e.Value?.ToString();

            return SelectedDisasterCardCodeChanged.InvokeAsync(
                string.IsNullOrWhiteSpace(value) ? null : value);
        }
    }
}
