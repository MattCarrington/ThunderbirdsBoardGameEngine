using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components
{
    public partial class BonusConditionList
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<BonusConditionViewModel> BonusConditions { get; set; } = [];

        [Parameter]
        public IReadOnlySet<string> SelectedBonusKeys { get; set; } = new HashSet<string>();

        [Parameter]
        public EventCallback<BonusConditionChanged> BonusChanged { get; set; }

        private Task OnChanged(string key, ChangeEventArgs e)
        {
            var selected = (bool)e.Value!;

            return BonusChanged.InvokeAsync(new BonusConditionChanged(key, selected));
        }
    }

    public record BonusConditionChanged(
        string Key,
        bool Selected
    );
}
