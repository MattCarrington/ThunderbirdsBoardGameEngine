using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Pages;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.Pages
{
    public class DisasterCardsTests : TestContext
    {
        private static IReadOnlyList<DisasterCardDto> Cards =>
        [
            new DisasterCardDto
            {
                Id = 1, Name = "Volcano", DifficultyNumber = 5, Location = "Pacific", RescueType = "Air",
                BonusConditions = [new BonusConditionDto { Description = "Have Fire Suit" }],
                Rewards = [new RewardDto { DisplayName = "2 Reputation" }]
            },
            new DisasterCardDto
            {
                Id = 2, Name = "Flood", DifficultyNumber = 3, Location = "Europe", RescueType = "Sea",
                BonusConditions = [],
                Rewards = []
            }
        ];

        [Fact]
        public void OnInitializedAsync_WhenDisasterCardsExist_LoadsCards()
        {
            // Arrange
            var service = SetupMockService(Cards);
            
            // Act
            var cut = RenderComponent<DisasterCards>();

            // Assert
            cut.WaitForElement("#disasterSelect option[value='1']"); // waits for card to render
            
            service.Received(1).GetAllAsync();
        }

        [Fact]
        public void Render_WhenCardsExist_OptionsAreAlphabetical()
        {
            // Arrange
            var cards = new[]
            {
                new DisasterCardDto { Id = 2, Name = "Beta" },
                new DisasterCardDto { Id = 1, Name = "Alpha" },
                new DisasterCardDto { Id = 3, Name = "Gamma" }
            };

            _ = SetupMockService(cards);

            // Act
            var cut = RenderComponent<DisasterCards>();

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
            var service = SetupMockService([]);

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await + Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("No disaster cards available.", cut.Markup);       // empty state text
                Assert.Empty(cut.FindAll("#disasterSelect"));                      // no dropdown
                Assert.DoesNotContain("Disaster Card Details", cut.Markup);        // no details panel
            });

            service.Received(1).GetAllAsync();
        }

        [Fact]
        public void Render_ShowsLoading_ThenSelectAppears()
        {
            // Arrange
            var tcs = new TaskCompletionSource<IReadOnlyList<DisasterCardDto>>(TaskCreationOptions.RunContinuationsAsynchronously);
            var service = Substitute.For<IDisasterCardService>();
            service.GetAllAsync().Returns(tcs.Task);

            Services.AddSingleton(service);

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Assert
            Assert.Contains("Loading cards...", cut.Markup);

            tcs.SetResult(Cards);
            cut.WaitForElement("select"); // now present
        }

        [Fact]
        public void Render_WhileFetching_ShowsLoadingThenHidesLoading()
        {
            // Arrange
            var tcs = new TaskCompletionSource<IReadOnlyList<DisasterCardDto>>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            var service = Substitute.For<IDisasterCardService>();
            service.GetAllAsync().Returns(_ => tcs.Task);

            Services.AddSingleton(service);

            // Act
            var cut = RenderComponent<DisasterCards>();
            Assert.Contains("Loading cards...", cut.Markup);

            tcs.SetResult(Cards);

            // Assert
            cut.WaitForElement("#disasterSelect");
            Assert.DoesNotContain("Loading cards...", cut.Markup);
        }

        [Fact]
        public void Select_CardWithBonusConditionAndRewards_ShowsConditionalSections()
        {
            // Arrange
            _ = SetupMockService(Cards);

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await
            var select = cut.WaitForElement("#disasterSelect");

            // Pick first card
            select.Change("1");
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
            _ = SetupMockService(Cards);

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await
            var select = cut.WaitForElement("#disasterSelect");

            // Assert
            select.Change("2");
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
            _ = SetupMockService(Cards);

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Await
            var select = cut.WaitForElement("#disasterSelect");

            // Assert
            select.Change("1");
            cut.WaitForAssertion(() => Assert.Contains("Disaster Card Details", cut.Markup));

            select.Change(string.Empty); // choose placeholder
            cut.WaitForAssertion(() => Assert.DoesNotContain("Disaster Card Details", cut.Markup));
        }
        
        [Fact]
        public void Select_ChangesSelection_DoesNotRefetch()
        {
            // Arrange
            var service = SetupMockService(Cards);

            var cut = RenderComponent<DisasterCards>();
            var select = cut.WaitForElement("#disasterSelect");

            // Act
            select.Change("1");
            select.Change("2");
            select.Change(string.Empty);

            // Assert
            service.Received(1).GetAllAsync(); // still only once
        }

        [Fact]
        public void Select_UnknownId_HidesDetails_NoCrash()
        {
            // Arrange
            _ = SetupMockService(Cards);

            var cut = RenderComponent<DisasterCards>();
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
                new DisasterCardDto { Id = 9, Name = "Nulls", DifficultyNumber = 1, Location = "Test", RescueType = "Air",
                                      BonusConditions = null, Rewards = null }
            };

            _ = SetupMockService(withNulls);

            var cut = RenderComponent<DisasterCards>();
            var select = cut.WaitForElement("#disasterSelect");
            select.Change("9");

            cut.WaitForAssertion(() =>
            {
                Assert.DoesNotContain("Bonus Conditions", cut.Markup);
                Assert.DoesNotContain("Rewards", cut.Markup);
            });
        }

        private IDisasterCardService SetupMockService(IReadOnlyList<DisasterCardDto> cards)
        {
            var service = Substitute.For<IDisasterCardService>();
            service.GetAllAsync().Returns(Task.FromResult(cards));

            Services.AddSingleton(service);

            return service;
        }
    }
}
