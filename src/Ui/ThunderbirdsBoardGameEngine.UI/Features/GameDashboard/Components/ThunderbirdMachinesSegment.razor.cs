using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components
{
    public partial class ThunderbirdMachinesSegment
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<ThunderbirdMachinesDashboardViewModel> Machines { get; set; } = [];

        [Parameter]
        public EventCallback<ThunderbirdMachinesDashboardViewModel> MoveRequested { get; set; }
    }
}
