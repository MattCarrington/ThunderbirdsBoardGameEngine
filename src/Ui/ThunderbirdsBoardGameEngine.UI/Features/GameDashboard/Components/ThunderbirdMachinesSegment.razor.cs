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

        [Parameter]
        public bool IsMutationPending { get; set; }

        [Parameter]
        public string ActiveCharacterCode { get; set; } = string.Empty;

        private string ActiveCharacterLocation => Machines.First(m => HasActiveCharacterAboard(m)).LocationCode;

        private bool CanTransferFrom(ThunderbirdMachinesDashboardViewModel source)
        {
            var activeMachine = Machines.FirstOrDefault(HasActiveCharacterAboard);

            return source.Occupants.Count > 0
                && activeMachine is not null
                && source.LocationCode == activeMachine.LocationCode
                && Machines.Any(machine =>
                    machine.ThunderbirdCode != source.ThunderbirdCode
                    && machine.LocationCode == source.LocationCode);
        }

        private bool CanMove(ThunderbirdMachinesDashboardViewModel machine)
        {
            return HasActiveCharacterAboard(machine);
        }

        private bool HasActiveCharacterAboard(ThunderbirdMachinesDashboardViewModel machine)
        {
            return machine.Occupants.Any(occupant => occupant.CharacterCode == ActiveCharacterCode);
        }
    }
}
