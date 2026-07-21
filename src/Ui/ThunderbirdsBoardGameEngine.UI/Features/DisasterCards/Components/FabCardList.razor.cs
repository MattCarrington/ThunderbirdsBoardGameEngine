using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components
{
    public partial class FabCardList
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<CardModifierViewModel> FabCards { get; set; } = [];

        [Parameter]
        public IReadOnlySet<string> SelectedFabCards { get; set; } = new HashSet<string>();

        [Parameter]
        public EventCallback<FabCardChanged> FabCardChanged { get; set; }

        private Task OnChanged(string key, ChangeEventArgs e)
        {
            var selected = (bool)e.Value!;

            return FabCardChanged.InvokeAsync(new FabCardChanged(key, selected));
        }
    }
}
