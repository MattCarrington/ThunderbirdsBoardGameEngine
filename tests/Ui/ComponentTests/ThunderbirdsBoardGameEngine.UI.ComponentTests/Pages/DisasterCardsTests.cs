using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Pages;
using ThunderbirdsBoardGameEngine.UI.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Pages
{
    public class DisasterCardsTests : BunitContext
    {
        private static IReadOnlyList<DisasterCardViewModel> Cards =>
        [
            new DisasterCardViewModel(
                Code: "volcano",
                DisplayName: "Volcano",
                DifficultyNumber: 5,
                Location: "Pacific",
                RescueType: "Air",
                BonusConditions: [
                    new BonusConditionViewModel(
                        Description: "Have fire suit",
                        Key: "have-fire-suit"
                    )
                ],
                Rewards: [
                    new RewardViewModel("teamwork")
                ]
            ),
            new DisasterCardViewModel(
                Code: "flood",
                DisplayName: "Flood",
                DifficultyNumber: 3,
                Location: "Europe",
                RescueType: "Sea",
                BonusConditions: [],
                Rewards: []
            )
        ];

        [Fact]
        public void OnInitializedAsync_WhenDisasterCardsExist_LoadsCards()
        {
            // Arrange
            var service = SetupDisasterCardService(Cards);
            _ = SetupCharactersService();
            _ = SetupRescueService();

            // Act
            var cut = Render<DisasterCards>();

            // Assert
            cut.WaitForElement("#disasterSelect option[value='volcano']"); // waits for card to render

            service.Received(1).GetAll();
        }

        [Fact]
        public void OnInitializedAsync_WhenCharactersExist_LoadsCharacters()
        {
            // Arrange
            _ = SetupDisasterCardService(Cards);
            var service = SetupCharactersService();
            _ = SetupRescueService();

            // Act
            var cut = Render<DisasterCards>();
            cut.Find("#disasterSelect").Change("volcano");

            // Assert
            cut.WaitForElement("#disasterSelect option[value='volcano']"); // waits for card to render
            cut.WaitForElement("#characterSelect option[value='scott']"); // waits for characters to render

            service.Received(1).GetAll();
        }

        [Fact]
        public void Render_WhenCardsExist_OptionsAreAlphabetical()
        {
            // Arrange
            var cards = new[]
            {
                new DisasterCardViewModel(
                    Code: "beta",
                    DisplayName: "Beta",
                    DifficultyNumber: 2,
                    Location: "Location B",
                    RescueType: "Type B",
                    BonusConditions: [],
                    Rewards: []
                ),
                new DisasterCardViewModel(
                    Code: "alpha",
                    DisplayName: "Alpha",
                    DifficultyNumber: 1,
                    Location: "Location A",
                    RescueType: "Type A",
                    BonusConditions: [],
                    Rewards: []
                ),
                new DisasterCardViewModel(
                    Code: "gamma",
                    DisplayName: "Gamma",
                    DifficultyNumber: 3,
                    Location: "Location C",
                    RescueType: "Type C",
                    BonusConditions: [],
                    Rewards: []
                )
            };

            _ = SetupDisasterCardService(cards);
            _ = SetupCharactersService();
            _ = SetupRescueService();

            // Act
            var cut = Render<DisasterCards>();

            // Await (wait for async load to complete)
            cut.WaitForElement("#disasterSelect");

            // Assert
            var optionTexts = cut
                .FindAll("#disasterSelect option")
                .Select(o => o.TextContent.Trim())
                .ToList();

            // First item is the placeholder
            Assert.Equal("-- Select a card --", optionTexts[0]);

            // The rest should be in alphabetical order
            var actualOrder = optionTexts.Skip(1).ToList();
            var expectedOrder = actualOrder.OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase).ToList();

            Assert.Equal(expectedOrder, actualOrder);
        }

        [Fact]
        public void Render_WhenNoDisasterCardsExist_DisplaysEmptyState()
        {
            // Arrange
            var service = SetupDisasterCardService([]);
            _ = SetupCharactersService();
            _ = SetupRescueService();

            // Act
            var cut = Render<DisasterCards>();

            // Await + Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("No disaster cards available.", cut.Markup);       // empty state text
                Assert.Empty(cut.FindAll("#disasterSelect"));                      // no dropdown
                Assert.DoesNotContain("Disaster Card Details", cut.Markup);        // no details panel
            });

            service.Received(1).GetAll();
        }

        [Fact]
        public void Select_CardWithBonusConditionAndRewards_ShowsConditionalSections()
        {
            // Arrange
            _ = SetupDisasterCardService(Cards);
            _ = SetupCharactersService();
            _ = SetupRescueService();

            // Act
            var cut = Render<DisasterCards>();

            // Await
            var select = cut.WaitForElement("#disasterSelect");

            // Pick first card
            select.Change("volcano");
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("Disaster Card Details", cut.Markup);
                Assert.Contains("Volcano", cut.Markup);
                Assert.Contains("<strong>Difficulty:</strong> 5", cut.Markup);
                Assert.Contains("<strong>Location:</strong> Pacific", cut.Markup);
                Assert.Contains("<strong>Rescue Type:</strong> Air", cut.Markup);
                Assert.Contains("Bonus Conditions", cut.Markup);
                Assert.Contains("Rewards", cut.Markup);
            });
        }

        [Fact]
        public void Select_CardWithoutBonusConditionAndRewards_HidesBonusConditionAndRewardsSections()
        {
            // Arrange
            _ = SetupDisasterCardService(Cards);
            _ = SetupCharactersService();
            _ = SetupRescueService();

            // Act
            var cut = Render<DisasterCards>();

            // Await
            var select = cut.WaitForElement("#disasterSelect");

            // Assert
            select.Change("flood");
            cut.WaitForAssertion(() =>
            {
                Assert.DoesNotContain("Bonus Conditions", cut.Markup);
                Assert.DoesNotContain("Rewards", cut.Markup);
            });
        }

        [Fact]
        public void Select_BlankSelection_HidesDetails()
        {
            // Arrange
            _ = SetupDisasterCardService(Cards);
            _ = SetupCharactersService();
            _ = SetupRescueService();

            // Act
            var cut = Render<DisasterCards>();

            // Await
            var select = cut.WaitForElement("#disasterSelect");

            // Assert
            select.Change("volcano");
            cut.WaitForAssertion(() => Assert.Contains("Disaster Card Details", cut.Markup));

            select.Change(string.Empty); // choose placeholder
            cut.WaitForAssertion(() => Assert.DoesNotContain("Disaster Card Details", cut.Markup));
        }

        [Fact]
        public void Select_ChangesSelection_DoesNotRefetch()
        {
            // Arrange
            var service = SetupDisasterCardService(Cards);
            _ = SetupCharactersService();
            _ = SetupRescueService();

            var cut = Render<DisasterCards>();
            var select = cut.WaitForElement("#disasterSelect");

            // Act
            select.Change("volcano");
            select.Change("flood");
            select.Change(string.Empty);

            // Assert
            service.Received(1).GetAll(); // still only once
        }

        [Fact]
        public void Select_UnknownId_HidesDetails_NoCrash()
        {
            // Arrange
            _ = SetupDisasterCardService(Cards);
            _ = SetupCharactersService();
            _ = SetupRescueService();

            var cut = Render<DisasterCards>();
            var select = cut.WaitForElement("#disasterSelect");

            // Act
            select.Change("9999"); // not in dictionary

            // Assert
            cut.WaitForAssertion(() =>
                Assert.DoesNotContain("Disaster Card Details", cut.Markup));
        }

        [Fact]
        public void SelectingCard_WithNullBonusAndRewards_HidesSections()
        {
            var withNulls = new[]
            {
                new DisasterCardViewModel
                (
                    Code: "nulls",
                    DisplayName: "Nulls",
                    DifficultyNumber: 1,
                    Location: "Test",
                    RescueType: "Air",
                    BonusConditions: null,
                    Rewards: null
                )
            };

            _ = SetupDisasterCardService(withNulls);
            _ = SetupCharactersService();
            _ = SetupRescueService();

            var cut = Render<DisasterCards>();
            var select = cut.WaitForElement("#disasterSelect");
            select.Change("nulls");

            cut.WaitForAssertion(() =>
            {
                Assert.DoesNotContain("Bonus Conditions", cut.Markup);
                Assert.DoesNotContain("Rewards", cut.Markup);
            });
        }

        [Fact]
        public void ClickingCalculate_WithSelectedBonuses_CallsRescueService()
        {
            // Arrange
            var cards = new[]
            {
                new DisasterCardViewModel
                (
                    Code: "test-card",
                    DisplayName: "Test Card",
                    DifficultyNumber: 4,
                    RescueType: "Air",
                    Location: "Test Location",
                    BonusConditions:
                    [
                        new BonusConditionViewModel (
                            Description: "Virgil",
                            Key: "character:virgil"
                        ),
                        new BonusConditionViewModel (
                            Description: "TB4",
                            Key: "thunderbird:tb4"
                        )
                    ],
                    Rewards: []
                )
            };

            var response = new CalculateRescueTargetResponseDto
            {
                TargetNumber = 6,
                TotalBonus = 2,
                AppliedDisasterBonuses = [
                    new AppliedDisasterBonusDto(){ BonusKey = "character:virgil", BonusValue = 2, SourceType = "disaster-card" }
                ]
            };

            _ = SetupDisasterCardService(cards);
            _ = SetupCharactersService();

            var rescueService = SetupRescueService(response);

            var cut = Render<DisasterCards>();

            // Select card
            cut.WaitForElement("#disasterSelect").Change("test-card");

            // Select character
            cut.Find("#characterSelect").Change("scott");

            // Check first checkbox
            var checkboxes = cut.FindAll("[data-testid='bonus-checkbox']");
            checkboxes[0].Change(true);

            // Act
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert
            rescueService.Received(1).CalculateRescueTargetAsync(
                "test-card",
                Arg.Is<IReadOnlyCollection<string>>(keys =>
                    keys.Count == 1 && keys.Contains("character:virgil")),
                "scott");
        }

        [Fact]
        public void Calculate_WhenServiceReturnsResult_DisplaysTarget()
        {
            // Arrange
            var response = new CalculateRescueTargetResponseDto
            {
                TargetNumber = 7,
                TotalBonus = 4,
                AppliedDisasterBonuses = []

            };

            _ = SetupDisasterCardService(Cards);
            _ = SetupCharactersService();
            var rescueService = SetupRescueService(response);

            Services.AddSingleton(rescueService);

            var cut = Render<DisasterCards>();

            cut.WaitForElement("#disasterSelect").Change("volcano");

            cut.WaitForElement("#characterSelect").Change("scott");

            // Act
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("Target Number", cut.Markup);
                Assert.Contains("7", cut.Markup);
                Assert.Contains("4", cut.Markup);
            });
        }

        [Fact]
        public void SelectingNewCard_ClearsBonusSelections()
        {
            // Arrange
            var cards = new[]
            {
                new DisasterCardViewModel(
                    Code: "card-one",
                    DisplayName: "Card One",
                    DifficultyNumber: 1,
                    Location: "Test",
                    RescueType: "Air",
                    BonusConditions:
                    [
                        new BonusConditionViewModel(
                            Description: "Bonus 1",
                            Key: "bonus1"
                        ),
                        new BonusConditionViewModel(
                            Description: "Bonus 2",
                            Key: "bonus2"
                        )
                    ],
                    Rewards: []
                ),
                new DisasterCardViewModel(
                    Code: "card-two",
                    DisplayName: "Card Two",
                    DifficultyNumber: 2,
                    Location: "Test",
                    RescueType: "Sea",
                    BonusConditions:
                    [
                        new BonusConditionViewModel(
                            Description: "Bonus 2",
                            Key: "bonus2"
                        )
                    ],
                    Rewards: []
                )
            };

            _ = SetupDisasterCardService(cards);
            _ = SetupCharactersService();
            _ = SetupRescueService();

            var cut = Render<DisasterCards>();

            var select = cut.WaitForElement("#disasterSelect");
            select.Change("card-one");

            var bonus2Checkbox = cut
                .FindAll("[data-testid='bonus-checkbox']")
                .Single(cb => cb.GetAttribute("data-bonus-key") == "bonus2");

            bonus2Checkbox.Change(true);

            // Act
            select.Change("card-two");

            // Assert
            cut.WaitForAssertion(() =>
                Assert.Empty(cut.FindAll("[data-testid='bonus-checkbox']:checked")));
        }

        [Fact]
        public void SelectingNewCard_ClearsPreviouslyCalculatedResult()
        {
            // Arrange
            var cards = new[]
            {
                new DisasterCardViewModel(
                    Code: "card-one",
                    DisplayName: "Card One",
                    DifficultyNumber: 1,
                    Location: "Test",
                    RescueType: "Air",
                    BonusConditions:
                    [
                        new BonusConditionViewModel(
                            Description: "Bonus 1",
                            Key: "bonus1"
                        )
                    ],
                    Rewards: []
                ),
                new DisasterCardViewModel(
                    Code: "card-two",
                    DisplayName: "Card Two",
                    DifficultyNumber: 2,
                    Location: "Test",
                    RescueType: "Sea",
                    BonusConditions:
                    [
                        new BonusConditionViewModel(
                            Description: "Bonus 2",
                            Key: "bonus2"
                        )
                    ],
                    Rewards: []
                )
            };

            _ = SetupDisasterCardService(cards);
            _ = SetupCharactersService();
            _ = SetupRescueService();

            var cut = Render<DisasterCards>();

            // Select first card
            var select = cut.WaitForElement("#disasterSelect");
            select.Change("card-one");

            cut.WaitForElement("#characterSelect").Change("scott");

            // Trigger calculation
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert result is shown
            cut.WaitForAssertion(() =>
                Assert.Contains("Target Number", cut.Markup));

            // Act — select a different card
            select.Change("card-two");

            // Assert — result is cleared
            cut.WaitForAssertion(() =>
                Assert.DoesNotContain("Target Number", cut.Markup));
        }

        [Fact]
        public void WhenNoCharactersExist_CharacterSelectHidden_AndCalculateDisabled()
        {
            _ = SetupDisasterCardService(Cards);
            _ = SetupRescueService();

            // Characters service returns empty
            _ = SetupCharactersService(Array.Empty<CharacterViewModel>());
            var cut = Render<DisasterCards>();

            cut.WaitForElement("#disasterSelect").Change("volcano");

            cut.WaitForAssertion(() =>
            {
                Assert.Empty(cut.FindAll("#characterSelect"));
                Assert.True(cut.Find("[data-testid='calculate-button']").HasAttribute("disabled"));
            });
        }

        [Fact]
        public void DeselectingCharacter_DisablesCalculate()
        {
            _ = SetupDisasterCardService(Cards);
            _ = SetupCharactersService();
            _ = SetupRescueService();

            var cut = Render<DisasterCards>();

            cut.WaitForElement("#disasterSelect").Change("volcano");

            var characterSelect = cut.WaitForElement("#characterSelect");
            characterSelect.Change("scott");

            Assert.False(cut.Find("[data-testid='calculate-button']").HasAttribute("disabled"));

            // Deselect (placeholder)
            characterSelect.Change(string.Empty);

            Assert.True(cut.Find("[data-testid='calculate-button']").HasAttribute("disabled"));
        }

        [Fact]
        public void SelectingCharacter_DoesNotClearSelectedBonuses()
        {
            var cards = new[]
            {
                new DisasterCardViewModel
                (
                    Code: "test-card",
                    DisplayName: "Test Card",
                    DifficultyNumber: 4,
                    RescueType: "Air",
                    Location: "Test Location",
                    BonusConditions:
                    [
                        new BonusConditionViewModel (
                            Description: "Virgil",
                            Key: "character:virgil"
                        )
                    ],
                    Rewards: []
                )
            };

            _ = SetupDisasterCardService(cards);
            _ = SetupCharactersService();
            _ = SetupRescueService();

            var cut = Render<DisasterCards>();

            cut.WaitForElement("#disasterSelect").Change("test-card");

            var bonus = cut.Find("[data-testid='bonus-checkbox']");
            bonus.Change(true);

            cut.Find("#characterSelect").Change("scott");

            Assert.True(bonus.HasAttribute("checked"));
        }

        [Fact]
        public void ChangingCharacter_DoesNotClearCalculatedResult()
        {
            _ = SetupDisasterCardService(Cards);
            _ = SetupCharactersService();
            _ = SetupRescueService(new CalculateRescueTargetResponseDto
            {
                TargetNumber = 5,
                TotalBonus = 1,
                AppliedDisasterBonuses = []
            });

            var cut = Render<DisasterCards>();

            cut.WaitForElement("#disasterSelect").Change("volcano");
            cut.Find("#characterSelect").Change("scott");
            cut.Find("[data-testid='calculate-button']").Click();

            cut.WaitForAssertion(() =>
                Assert.Contains("Target Number", cut.Markup));

            // Change character
            cut.Find("#characterSelect").Change("virgil");

            Assert.Contains("Target Number", cut.Markup);
        }

        private IDisasterCardService SetupDisasterCardService(IReadOnlyList<DisasterCardViewModel> cards)
        {
            var service = Substitute.For<IDisasterCardService>();
            service.GetAll().Returns(cards);
            service.GetByCode(Arg.Any<string>()).Returns(args =>
            {
                var code = args[0] as string;
                return cards.SingleOrDefault(c => c.Code == code);
            });

            Services.AddSingleton(service);

            return service;
        }

        private ICharacterService SetupCharactersService()
        {
            var characters = new List<CharacterViewModel>
            {
                new(Key: "scott", DisplayName: "Scott"),
                new(Key: "virgil", DisplayName: "Virgil"),
            };

            return SetupCharactersService(characters);
        }

        private ICharacterService SetupCharactersService(IReadOnlyList<CharacterViewModel> characters)
        {
            var service = Substitute.For<ICharacterService>();
            service.GetAll().Returns(characters);

            Services.AddSingleton(service);

            return service;
        }

        private IRescueService SetupRescueService()
        {
            var dto = new CalculateRescueTargetResponseDto
            {
                TargetNumber = 5,
                TotalBonus = 0,
                AppliedDisasterBonuses = []
            };

            return SetupRescueService(dto);
        }

        private IRescueService SetupRescueService(CalculateRescueTargetResponseDto dto)
        {
            var service = Substitute.For<IRescueService>();
            service.CalculateRescueTargetAsync(Arg.Any<string>(), Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<string>())
                .Returns(Task.FromResult<CalculateRescueTargetResponseDto?>(dto));

            Services.AddSingleton(service);

            return service;
        }
    }
}
