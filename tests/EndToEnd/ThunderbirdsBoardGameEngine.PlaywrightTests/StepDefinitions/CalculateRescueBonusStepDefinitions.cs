using System;
using System.Threading.Tasks;
using Reqnroll;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Pages;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.StepDefinitions
{
    [Binding]
    public class CalculateRescueBonusStepDefinitions
    {
        private readonly DisasterCardsPage _page;

        public CalculateRescueBonusStepDefinitions(DisasterCardsPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
        }

        [Given("the disaster {string} is selected")]
        public async Task GivenTheDisasterIsSelected(string cardName)
        {
            await _page.GotoAsync();
            await _page.SelectCardAsync(cardName);
        }

        [Given("{string} is selected as performing the rescue")]
        public async Task GivenIsSelectedAsPerformingTheRescue(string character)
        {
            await _page.SelectCharacterAsync(character);
        }

        [Given("the {string} bonus is marked as being present")]
        public async Task GivenTheBonusIsMarkedAsBeingPresent(string bonusName)
        {
            await _page.MarkBonusCheckboxAsync(bonusName);
        }

        [When("the rescue target is calculated")]
        public async Task WhenTheRescueTargetIsCalculated()
        {
            await _page.ClickCalculateButton();
        }

        [Then("the minimum required roll should be {int}")]
        public async Task ThenTheMinimumRequiredRollShouldBe(int targetRoll)
        {
            await _page.AssertRescueResultDisplayedAsync(targetRoll);
        }

        [Then("the rescue tarrget should be displayed")]
        public async Task ThenTheRescueTarrgetShouldBeDisplayed()
        {
            await _page.AssertRescueResultDisplayedAsync();
        }
    }
}
