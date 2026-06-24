using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components
{
    public partial class EventCardList
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<CardModifierViewModel> EventCards { get; set; } = [];

        [Parameter]
        public IReadOnlySet<string> SelectedEventCards { get; set; } = new HashSet<string>();

        [Parameter]
        public EventCallback<EventCardChanged> EventCardChanged { get; set; }

        private Task OnChanged(string key, ChangeEventArgs e)
        {
            var selected = (bool)e.Value!;

            return EventCardChanged.InvokeAsync(new EventCardChanged(key, selected));
        }
    }
}
