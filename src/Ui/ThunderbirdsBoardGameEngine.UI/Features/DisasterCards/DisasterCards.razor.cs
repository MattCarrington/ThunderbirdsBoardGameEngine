using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
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

        private IReadOnlyList<DisasterCardViewModel>? cards;
        private IReadOnlyList<CharacterViewModel>? characters;

        private string? selectedCardCode;
        private DisasterCardViewModel? selectedCard;
        private string? selectedCharacter;

        private readonly HashSet<string> selectedBonusKeys = new();

        private CalculateRescueTargetResponseDto? calculationResult;
        private bool calculationFailed;
        private bool isCalculating;

        private bool IsCalculateDisabled =>
            isCalculating
            || selectedCard is null
            || string.IsNullOrWhiteSpace(selectedCharacter);

        protected override void OnInitialized()
        {
            var allCards = DisasterCardService.GetAll();
            cards = allCards.OrderBy(c => c.DisplayName ?? string.Empty, StringComparer.OrdinalIgnoreCase).ToList();

            var allCharacters = CharacterService.GetAll();
            characters = allCharacters.OrderBy(c => c.DisplayName ?? string.Empty, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private string? SelectedCardId
        {
            get => selectedCardCode;
            set
            {
                if (selectedCardCode == value)
                {
                    return;
                }

                selectedCardCode = value;
                selectedBonusKeys.Clear();
                calculationResult = null;

                selectedCard = !string.IsNullOrWhiteSpace(value)
                    ? DisasterCardService.GetByCode(value)
                    : null;
            }
        }

        private void OnBonusToggled(string key, ChangeEventArgs e)
        {
            var isChecked = (bool)e.Value!;

            if (isChecked)
            {
                selectedBonusKeys.Add(key);
            }
            else
            {
                selectedBonusKeys.Remove(key);
            }
        }

        private async Task CalculateRescueTarget()
        {
            if (selectedCard is null || string.IsNullOrWhiteSpace(selectedCharacter))
            {
                return;
            }

            isCalculating = true;
            calculationFailed = false;
            calculationResult = null;

            try
            {
                calculationResult =
                    await RescueService.CalculateRescueTargetAsync(
                        selectedCard.Code,
                        selectedBonusKeys,
                        selectedCharacter);

                calculationFailed = calculationResult is null;
            }
            catch
            {
                // HTTP error or other exception occurred
                calculationFailed = true;
            }
            finally
            {
                isCalculating = false;
            }
        }
    }
}
