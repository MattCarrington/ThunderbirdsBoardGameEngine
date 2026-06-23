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

        [Given("the player selects the character {string}")]
        public async Task GivenIsSelectedAsPerformingTheRescue(string character)
        {
            await _page.SelectCharacterAsync(character);
        }

        [Given("the player marks {string} as being present")]
        public async Task GivenTheBonusIsMarkedAsBeingPresent(string bonusName)
        {
            await _page.MarkBonusCheckboxAsync(bonusName);
        }

        [Given("the player marks the F.A.B. card {string} as being played")]
        public async Task GivenThePlayerMarksTheFabCardAsBeingPlayed(string cardName)
        {
            await _page.MarkFabCardCheckboxAsync(cardName);
        }

        [Given("the player marks the event card {string} as being active")]
        public async Task GivenThePlayerMarksTheEventCardAsBeingActive(string cardName)
        {
            await _page.MarkEventCardCheckboxAsync(cardName);
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

        [Then("the rescue target should be displayed")]
        public async Task ThenTheRescueTargetShouldBeDisplayed()
        {
            await _page.AssertRescueResultDisplayedAsync();
        }
    }
}
