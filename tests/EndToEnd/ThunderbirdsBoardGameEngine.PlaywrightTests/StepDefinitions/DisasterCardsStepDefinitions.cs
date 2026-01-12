using Reqnroll;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Pages;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.StepDefinitions
{
    [Binding]
    public class DisasterCardsStepDefinitions
    {
        private string? _selectedCardName;
        private readonly DisasterCardsPage _page;

        public DisasterCardsStepDefinitions(DisasterCardsPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
        }

        [Given("I navigate to the disaster cards page")]
        public async Task GivenINavigateToTheDisasterCardsPageAsync()
        {
            await _page.GotoAsync();
        }

        [Then("the dropdown contains a list of Disaster Cards")]
        public async Task ThenTheDropdownContainsAListOfDisasterCardsAsync()
        {
            await _page.AssertHasAnyCardsAsync();
        }

        [When(@"I select the disaster card ""(.*)""")]
        public async Task WhenISelectTheDisasterCard(string cardName)
        {
            _selectedCardName = cardName;
            await _page.SelectCardAsync(_selectedCardName);
        }

        [Then("the selected card details are displayed")]
        public async Task ThenTheSelectedCardDetailsAreDisplayed()
        {
            await _page.AssertCardDetailsVisibleAsync(_selectedCardName!);
        }
    }
}
