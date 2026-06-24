using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards
{
    public partial class DisasterCardsPage
    {
        [Inject]
        private IDisasterCardService DisasterCardService { get; set; } = null!;

        [Inject]
        private ICharacterService CharacterService { get; set; } = null!;

        [Inject]
        private IRescueClientService RescueService { get; set; } = null!;

        [Inject]
        private IRescueCalculationModifierService CardModifierService { get; set; } = null!;

        private IReadOnlyList<DisasterCardSummaryViewModel> _cards = Array.Empty<DisasterCardSummaryViewModel>();
        private IReadOnlyList<CharacterViewModel> _characters = Array.Empty<CharacterViewModel>();
        private IReadOnlyList<CardModifierViewModel> _fabCardModifiers = Array.Empty<CardModifierViewModel>();
        private IReadOnlyList<CardModifierViewModel> _eventCardModifiers = Array.Empty<CardModifierViewModel>();

        private string? _selectedCardCode = string.Empty;
        private DisasterCardViewModel? _selectedCard;
        private string? _selectedCharacter = string.Empty;

        private HashSet<string> _selectedBonusKeys = new();
        private HashSet<string> _selectedFabCardKeys = new();
        private HashSet<string> _selectedEventCardKeys = new();

        private CalculateRescueTargetResponseDto? _calculationResult;
        private bool _calculationFailed;
        private bool _isCalculating;

        private bool IsCalculateDisabled =>
            _isCalculating
            || _selectedCard is null
            || string.IsNullOrWhiteSpace(_selectedCharacter);

        protected override void OnInitialized()
        {
            _cards = DisasterCardService.GetAll();
            _characters = CharacterService.GetAll();
            _fabCardModifiers = CardModifierService.GetFabCards();
            _eventCardModifiers = CardModifierService.GetEventCards();
        }

        private void OnDisasterCardChanged(string? disasterCard)
        {
            _selectedCardCode = disasterCard;
            _selectedBonusKeys.Clear();
            _calculationResult = null;

            _selectedCard = !string.IsNullOrWhiteSpace(disasterCard)
                ? DisasterCardService.GetByCode(disasterCard)
                : null;
        }

        private void OnBonusToggled(BonusConditionChanged change)
        {
            if (change.Selected)
            {
                _selectedBonusKeys.Add(change.Key);
            }
            else
            {
                _selectedBonusKeys.Remove(change.Key);
            }

            ClearCalculationState();
        }

        private async Task CalculateRescueTarget()
        {
            if (_selectedCard is null || string.IsNullOrWhiteSpace(_selectedCharacter))
            {
                return;
            }

            _isCalculating = true;

            ClearCalculationState();

            try
            {
                _calculationResult =
                    await RescueService.CalculateRescueTargetAsync(
                        _selectedCard.Code,
                        _selectedBonusKeys,
                        _selectedCharacter,
                        _selectedFabCardKeys,
                        _selectedEventCardKeys);

                _calculationFailed = _calculationResult is null;
            }
            catch
            {
                // HTTP error or other exception occurred
                _calculationFailed = true;
            }
            finally
            {
                _isCalculating = false;
            }
        }

        private void OnCharacterChanged(string? characterCode)
        {
            _selectedCharacter = characterCode;

            ClearCalculationState();
        }

        private void OnFabCardChanged(FabCardChanged change)
        {
            if (change.Selected)
            {
                _selectedFabCardKeys.Add(change.Key);
            }
            else
            {
                _selectedFabCardKeys.Remove(change.Key);
            }

            ClearCalculationState();
        }

        private void OnEventCardChanged(EventCardChanged change)
        {
            if (change.Selected)
            {
                _selectedEventCardKeys.Add(change.Key);
            }
            else
            {
                _selectedEventCardKeys.Remove(change.Key);
            }

            ClearCalculationState();
        }

        private void ClearCalculationState()
        {
            _calculationResult = null;
            _calculationFailed = false;
        }
    }
}
