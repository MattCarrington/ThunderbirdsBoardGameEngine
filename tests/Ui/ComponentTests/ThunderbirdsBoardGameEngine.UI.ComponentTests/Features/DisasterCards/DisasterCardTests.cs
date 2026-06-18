using Bunit;
using NSubstitute;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.DisasterCards
{
    public class DisasterCardTests : BunitContext
    {
        [Fact]
        public void CallsDisasterServiceOnInitialization()
        {
            // Arrange
            var context = CreateTestContext();

            // Act
            _ = Render<DisasterCardsPage>();

            // Assert
            context.DisasterCardService.Received(1).GetAll();
        }

        [Fact]
        public void CallsCharacterServiceOnInitialization()
        {
            // Arrange
            var context = CreateTestContext();

            // Act
            _ = Render<DisasterCardsPage>();

            // Assert
            context.CharacterService.Received(1).GetAll();
        }

        [Fact]
        public void DisplaysDisasterCardDetailsWhenSelected()
        {
            // Arrange
            var context = CreateTestContext();

            var cut = Render<DisasterCardsPage>();

            // Act
            cut.Find("#disasterSelect").Change("DC1");

            // Assert
            Assert.Contains("Disaster Card Details", cut.Markup);
        }

        [Fact]
        public void HidesDisasterCardDetailsWhenNotSelected()
        {
            // Arrange
            var context = CreateTestContext();

            var cut = Render<DisasterCardsPage>();

            var select = cut.Find("#disasterSelect");

            select.Change("DC1"); // Select a disaster card
            Assert.Contains("Disaster Card Details", cut.Markup); // Verify details are displayed

            // Act
            select.Change(""); // Deselect the disaster card

            // Assert
            Assert.DoesNotContain(cut.Markup, "Disaster Card Details");
        }

        [Fact]
        public void CallsRescueClientServiceWhenCalculateClicked()
        {
            // Arrange
            var context = CreateTestContext();
            var cut = Render<DisasterCardsPage>();

            // Select disaster card and character
            cut.Find("#disasterSelect").Change("DC1");
            cut.Find("#characterSelect").Change("C1");

            // Act
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert
            context.RescueClientService.Received(1).CalculateRescueTargetAsync(
                disasterCardCode: Arg.Is<string>(x => x == "DC1"),
                presentBonusKeys: Arg.Any<IReadOnlyCollection<string>>(),
                performingCharacterKey: Arg.Is<string>(x => x == "C1")
            );
        }

        [Fact]
        public void CalculateButtonIsDisabledWhenNoCharacterSelected()
        {
            // Arrange
            var context = CreateTestContext();

            var cut = Render<DisasterCardsPage>();

            // Act
            cut.Find("#disasterSelect").Change("DC1");

            // Assert
            var validateButton = cut.Find("button");
            Assert.True(validateButton.HasAttribute("disabled"));
        }

        [Fact]
        public void DisplaysTargetNumberAndTotalBonusWhenCalculationSucceeds()
        {
            // Arrange
            var context = CreateTestContext();

            var response = CreateRescueCalculationResponse();

            SetupRescueClientService(context, response);

            var cut = Render<DisasterCardsPage>();

            cut.Find("#disasterSelect").Change("DC1");
            cut.Find("#characterSelect").Change("C1");

            // Act
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                cut.Find("[data-testid='rescue-calculation-result']");
            });
        }

        [Fact]
        public void DisplaysErrorMessageWhenCalculationFails()
        {
            // Arrange
            var context = CreateTestContext();

            SetupRescueClientService(context, null); // Simulate failure

            var cut = Render<DisasterCardsPage>();

            cut.Find("#disasterSelect").Change("DC1");
            cut.Find("#characterSelect").Change("C1");

            // Act
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                cut.Find("[data-testid='rescue-calculation-error']");
            });
        }

        [Fact]
        public void ClearsCalculatedResultWhenDisasterCardChanges()
        {
            // Arrange
            var context = CreateTestContext();

            var response = CreateRescueCalculationResponse();

            SetupRescueClientService(context, response);

            var cut = Render<DisasterCardsPage>();

            cut.Find("#disasterSelect").Change("DC1");
            cut.Find("#characterSelect").Change("C1");

            cut.Find("[data-testid='calculate-button']").Click();

            cut.WaitForAssertion(() =>
            {
                cut.Find("[data-testid='rescue-calculation-result']");
            });

            // Act
            cut.Find("#disasterSelect").Change("DC2");

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Throws<ElementNotFoundException>(() => cut.Find("[data-testid='rescue-calculation-result']"));
                Assert.Throws<ElementNotFoundException>(() => cut.Find("[data-testid='rescue-calculation-error']"));
            });
        }

        [Fact]
        public void ClearsCalculatedResultWhenCharacterChanges()
        {
            // Arrange
            var context = CreateTestContext();

            var response = CreateRescueCalculationResponse();

            SetupRescueClientService(context, response);

            var cut = Render<DisasterCardsPage>();

            cut.Find("#disasterSelect").Change("DC1");
            cut.Find("#characterSelect").Change("C1");

            cut.Find("[data-testid='calculate-button']").Click();

            cut.WaitForAssertion(() =>
            {
                cut.Find("[data-testid='rescue-calculation-result']");
            });

            // Act
            cut.Find("#characterSelect").Change("C2");

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Throws<ElementNotFoundException>(() => cut.Find("[data-testid='rescue-calculation-result']"));
                Assert.Throws<ElementNotFoundException>(() => cut.Find("[data-testid='rescue-calculation-error']"));
            });
        }

        [Fact]
        public void ClearsCalculatedResultWhenBonusToggled()
        {
            // Arrange
            var context = CreateTestContext();

            var response = CreateRescueCalculationResponse();

            SetupRescueClientService(context, response);

            var cut = Render<DisasterCardsPage>();

            cut.Find("#disasterSelect").Change("DC1");
            cut.Find("#characterSelect").Change("C1");

            cut.Find("[data-testid='calculate-button']").Click();

            cut.WaitForAssertion(() =>
            {
                cut.Find("[data-testid='rescue-calculation-result']");
            });

            // Act
            var bonusCheckbox = cut
                .FindAll("[data-testid='bonus-checkbox']")
                .Single(cb => cb.GetAttribute("data-bonus-key") == "bonus2");

            bonusCheckbox.Change(true);

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Throws<ElementNotFoundException>(() => cut.Find("[data-testid='rescue-calculation-result']"));
                Assert.Throws<ElementNotFoundException>(() => cut.Find("[data-testid='rescue-calculation-error']"));
            });
        }

        private DisasterCardPageTestContext CreateTestContext()
        {
            var context = new DisasterCardPageTestContext(this);

            var disasterCards = CreateDisasterCards();

            context.DisasterCardService.GetAll().Returns(disasterCards.Select(d => new DisasterCardSummaryViewModel(d.Code, d.DisplayName)).ToList());
            context.DisasterCardService.GetByCode("DC1").Returns(disasterCards.First());
            context.CharacterService.GetAll().Returns(CreateCharacters());

            return context;
        }

        private void SetupRescueClientService(DisasterCardPageTestContext context, CalculateRescueTargetResponseDto? response)
        {
            context.RescueClientService.CalculateRescueTargetAsync(
                Arg.Any<string>(),
                Arg.Any<IReadOnlyCollection<string>>(),
                Arg.Any<string>())
                .Returns(response);
        }

        private static CalculateRescueTargetResponseDto CreateRescueCalculationResponse()
        {
            return new CalculateRescueTargetResponseDto
            {
                TargetNumber = 10,
                TotalBonus = 5,
                AppliedDisasterBonuses = Array.Empty<AppliedDisasterBonusDto>()
            };
        }

        private static IReadOnlyList<DisasterCardViewModel> CreateDisasterCards()
        {
            return
            [
                new(Code: "DC1", DisplayName: "Disaster Card 1", DifficultyNumber: 1, Location: "Location 1", RescueType: "Type 1", BonusConditions: [new BonusConditionViewModel(Description: "Bonus 1", Key: "bonus1"), new BonusConditionViewModel(Description: "Bonus 2", Key: "bonus2")], Rewards: []),
                new(Code: "DC2", DisplayName: "Disaster Card 2", DifficultyNumber: 2, Location: "Location 2", RescueType: "Type 2", BonusConditions: [], Rewards: [])
            ];
        }

        private static IReadOnlyList<CharacterViewModel> CreateCharacters()
        {
            return
            [
                new(Key: "C1", DisplayName: "Character 1"),
                new(Key: "C2", DisplayName: "Character 2")
            ];
        }
    }
}
