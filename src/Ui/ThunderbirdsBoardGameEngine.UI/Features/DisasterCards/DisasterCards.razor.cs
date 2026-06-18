using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards
{
    public partial class DisasterCards
    {
        [Inject]
        private IDisasterCardService DisasterCardService { get; set; } = null!;

        [Inject]
        private ICharacterService CharacterService { get; set; } = null!;

        [Inject]
        private IRescueService RescueService { get; set; } = null!;

        private IReadOnlyList<DisasterCardViewModel> _cards = Array.Empty<DisasterCardViewModel>();
        private IReadOnlyList<CharacterViewModel> _characters = Array.Empty<CharacterViewModel>();

        private string? _selectedCardCode = string.Empty;
        private DisasterCardViewModel? _selectedCard;
        private string? _selectedCharacter = string.Empty;

        private HashSet<string> _selectedBonusKeys = new();

        private CalculateRescueTargetResponseDto? _calculationResult;
        private bool _calculationFailed;
        private bool _isCalculating;

        private bool IsCalculateDisabled =>
            _isCalculating
            || _selectedCard is null
            || string.IsNullOrWhiteSpace(_selectedCharacter);

        protected override void OnInitialized()
        {
            var allCards = DisasterCardService.GetAll();
            _cards = allCards.OrderBy(c => c.DisplayName ?? string.Empty, StringComparer.OrdinalIgnoreCase).ToList();

            _characters = CharacterService.GetAll();
        }

        private void OnDisasterCardChanged(string disasterCard)
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
        }

        private async Task CalculateRescueTarget()
        {
            if (_selectedCard is null || string.IsNullOrWhiteSpace(_selectedCharacter))
            {
                return;
            }

            _isCalculating = true;
            _calculationFailed = false;
            _calculationResult = null;

            try
            {
                _calculationResult =
                    await RescueService.CalculateRescueTargetAsync(
                        _selectedCard.Code,
                        _selectedBonusKeys,
                        _selectedCharacter);

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

        private void OnCharacterChanged(string characterCode)
        {
            _selectedCharacter = characterCode;
            _calculationResult = null;
        }
    }
}
