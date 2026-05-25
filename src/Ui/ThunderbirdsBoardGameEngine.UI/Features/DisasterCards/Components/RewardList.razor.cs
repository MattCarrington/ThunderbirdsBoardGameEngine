using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components
{
    public partial class RewardList
    {
        [Parameter]
        public IReadOnlyList<RewardViewModel> Rewards { get; set; } = [];
    }
}
