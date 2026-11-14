using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Components
{
    public partial class ThunderbirdMachinesSegment
    {
        [Parameter, EditorRequired]
        public IReadOnlyList<ThunderbirdMachinesDashboardViewModel> Machines { get; set; } = [];

        [Parameter]
        public EventCallback<ThunderbirdMachinesDashboardViewModel> MoveRequested { get; set; }

        [Parameter]
        public EventCallback<ThunderbirdMachinesDashboardViewModel> TransferRequested { get; set; }

        private bool CanTransferFrom(ThunderbirdMachinesDashboardViewModel source)
        {
            return source.Occupants.Count > 0
                && Machines.Any(destination => destination.ThunderbirdCode != source.ThunderbirdCode
                    && destination.LocationCode == source.LocationCode);
        }

        private static bool CanMove(ThunderbirdMachinesDashboardViewModel machine)
        {
            return machine.Occupants.Count > 0;
        }
    }
}
