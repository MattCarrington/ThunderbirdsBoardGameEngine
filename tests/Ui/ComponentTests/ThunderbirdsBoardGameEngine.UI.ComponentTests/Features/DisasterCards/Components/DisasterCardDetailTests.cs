using Bunit;
using System.Threading.Channels;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.DisasterCards.Components
{
    public class DisasterCardDetailTests : BunitContext
    {
        [Fact]
        public void ShouldRenderDisasterCardDetails()
        {
            // Arrange
            var disasterCard = CreateDisasterCardViewModel("fire", "Fire", 1, "Land", "North America");

            // Act
            var cut = Render<DisasterCardDetails>(parameters => parameters.Add(p => p.Card, disasterCard));

            // Assert
            Assert.Contains("Fire", cut.Markup);
            Assert.Contains("Difficulty", cut.Markup);
            Assert.Contains("Rescue Type", cut.Markup);
            Assert.Contains("Location", cut.Markup);
        }

        [Fact]
        public void ShouldRenderBonusConditions()
        {
            // Arrange
            var disasterCard = CreateDisasterCardViewModel("flood", "Flood", 2, "Water", "Europe");

            // Act
            var cut = Render<DisasterCardDetails>(parameters => parameters.Add(p => p.Card, disasterCard));

            // Assert
            Assert.Contains("Extra points for quick response", cut.Markup);
        }

        [Fact]
        public void ShouldRenderRewards()
        {
            // Arrange
            var disasterCard = CreateDisasterCardViewModel("earthquake", "Earthquake", 3, "Land", "Asia");

            // Act
            var cut = Render<DisasterCardDetails>(parameters => parameters.Add(p => p.Card, disasterCard));

            // Assert
            Assert.Contains("Reward 1", cut.Markup);
            Assert.Contains("Reward 2", cut.Markup);
        }

        [Fact]
        public void ShouldNotifyParentOnBonusSelected()
        {
            // Arrange
            var disasterCard = CreateDisasterCardViewModel("storm", "Storm", 4, "Air", "Australia");

            BonusConditionChanged? changed = null;

            var cut = Render<DisasterCardDetails>(parameters => parameters
                .Add(p => p.Card, disasterCard)
                .Add(p => p.SelectedBonusKeys, new HashSet<string>())
                .Add(p => p.BonusChanged, value => changed = value));

            // Act
            cut.Find("[data-bonus-key='extra-points']").Change(true);

            // Assert
            Assert.Equal(new BonusConditionChanged("extra-points", true), changed);
        }

        [Fact]
        public void ShouldNotifyParentOnCharacterChanged()
        {
            // Arrange
            var disasterCard = CreateDisasterCardViewModel("tsunami", "Tsunami", 5, "Water", "Japan");

            var characters = CreateSampleCharacters();

            string? selectedCharacterCode = null;

            var cut = Render<DisasterCardDetails>(parameters => parameters
                .Add(p => p.Card, disasterCard)
                .Add(p => p.SelectedCharacterCode, null)
                .Add(p => p.SelectedCharacterCodeChanged, value => selectedCharacterCode = value)
                .Add(p => p.Characters, characters));

            // Act
            cut.Find("#characterSelect").Change("character-2");

            // Assert
            Assert.Equal("character-2", selectedCharacterCode);
        }

        [Fact]
        public void ShouldNotifyParentOnCalculateClicked()
        {
            // Arrange
            var disasterCard = CreateDisasterCardViewModel("volcano", "Volcano", 6, "Land", "South America");

            bool calculateClicked = false;

            var cut = Render<DisasterCardDetails>(parameters => parameters
                .Add(p => p.Card, disasterCard)
                .Add(p => p.CalculateClicked, () => calculateClicked = true));

            // Act
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert
            Assert.True(calculateClicked);
        }

        [Fact]
        public void CalculateButtonShouldBeDisabledWhenIsCalculationDisabledIsTrue()
        {
            // Arrange
            var disasterCard = CreateDisasterCardViewModel("hurricane", "Hurricane", 7, "Air", "Caribbean");

            var cut = Render<DisasterCardDetails>(parameters => parameters
                .Add(p => p.Card, disasterCard)
                .Add(p => p.IsCalculationDisabled, true));

            // Act
            var button = cut.Find("[data-testid='calculate-button']");

            // Assert
            Assert.True(button.HasAttribute("disabled"));
        }

        [Fact]
        public void DisplaysCalculationResultWhenCalculationResultIsNotNull()
        {
            // Arrange
            var disasterCard = CreateDisasterCardViewModel("blizzard", "Blizzard", 8, "Air", "Canada");

            var calculationResult = new CalculateRescueTargetResponseDto
            {
                TargetNumber = 42,
                TotalBonus = 5,
                AppliedDisasterBonuses = new List<AppliedDisasterBonusDto>
                {
                    new AppliedDisasterBonusDto
                    {
                        BonusKey = "extra-points",
                        BonusValue = 5,
                        SourceType = "DisasterCard"
                    }
                }
            };

            var cut = Render<DisasterCardDetails>(parameters => parameters
                .Add(p => p.Card, disasterCard)
                .Add(p => p.CalculationResult, calculationResult));

            // Act
            var result = cut.Find("[data-testid='rescue-calculation-result']");

            // Assert
            Assert.Contains("42", result.InnerHtml);
        }

        [Fact]
        public void DoesNotDisplayCalculationResultWhenCalculationResultIsNull()
        {
            // Arrange
            var disasterCard = CreateDisasterCardViewModel("blizzard", "Blizzard", 8, "Air", "Canada");
            var cut = Render<DisasterCardDetails>(parameters => parameters
                .Add(p => p.Card, disasterCard)
                .Add(p => p.CalculationResult, null));

            // Act
            var resultElements = cut.FindAll("[data-testid='rescue-calculation-result']");

            // Assert
            Assert.Empty(resultElements);
        }

        [Fact]
        public void DisplaysCalculationFailedMessageWhenCalculationFailedIsTrue()
        {
            // Arrange
            var disasterCard = CreateDisasterCardViewModel("blizzard", "Blizzard", 8, "Air", "Canada");

            var cut = Render<DisasterCardDetails>(parameters => parameters
                .Add(p => p.Card, disasterCard)
                .Add(p => p.CalculationFailed, true));

            // Act
            var failedMessageElements = cut.FindAll("[data-testid='rescue-calculation-error']");

            // Assert
            Assert.NotEmpty(failedMessageElements);
        }

        private static DisasterCardViewModel CreateDisasterCardViewModel(string code, string displayName, int difficultyNumber, string rescueType, string location)
        {
            return new DisasterCardViewModel(
                Code: code,
                DisplayName: displayName,
                DifficultyNumber: difficultyNumber,
                RescueType: rescueType,
                Location: location,
                BonusConditions: [
                    new BonusConditionViewModel(Description: "Extra points for quick response", Key: "extra-points")
                ],
                Rewards: [
                    new RewardViewModel(Description: "Reward 1"),
                    new RewardViewModel(Description: "Reward 2")
                ]
            );
        }

        private static IReadOnlyList<CharacterViewModel> CreateSampleCharacters()
        {
            return
            [
                new(Key: "character-1", DisplayName: "Character 1"),
                new(Key: "character-2", DisplayName: "Character 2"),
                new(Key: "character-3", DisplayName: "Character 3")
            ];
        }
    }
}
