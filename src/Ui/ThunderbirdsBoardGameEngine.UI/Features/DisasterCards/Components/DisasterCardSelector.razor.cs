using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components
{
    public partial class DisasterCardSelector
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<DisasterCardViewModel> DisasterCards { get; set; } = [];

        [Parameter]
        public string? SelectedDisasterCardCode { get; set; }

        [Parameter]
        public EventCallback<string?> SelectedDisasterCardCodeChanged { get; set; }

        private Task OnChanged(ChangeEventArgs e)
        {
            return SelectedDisasterCardCodeChanged.InvokeAsync(e.Value?.ToString());
        }
    }
}
