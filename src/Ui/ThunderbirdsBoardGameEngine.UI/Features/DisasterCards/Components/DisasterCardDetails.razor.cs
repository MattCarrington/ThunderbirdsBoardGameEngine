using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components
{
    public partial class DisasterCardDetails
    {
        [Parameter]
        public DisasterCardViewModel Card { get; set; } = null!;

        [Parameter]
        public IReadOnlyList<CharacterViewModel> Characters { get; set; } = [];

        [Parameter]
        public string? SelectedCharacterCode { get; set; }

        [Parameter]
        public IReadOnlySet<string> SelectedBonusKeys { get; set; } = new HashSet<string>();

        [Parameter]
        public CalculateRescueTargetResponseDto? CalculationResult { get; set; }

        [Parameter]
        public bool CalculationFailed { get; set; }

        [Parameter]
        public bool IsCalculationDisabled { get; set; }

        [Parameter]
        public EventCallback<string?> SelectedCharacterCodeChanged { get; set; }

        [Parameter]
        public EventCallback<BonusConditionChanged> BonusChanged { get; set; }

        [Parameter]
        public EventCallback CalculateClicked { get; set; }
    }
}
