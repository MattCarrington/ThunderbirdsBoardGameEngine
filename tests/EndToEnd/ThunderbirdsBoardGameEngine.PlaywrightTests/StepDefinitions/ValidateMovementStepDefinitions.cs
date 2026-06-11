using Reqnroll;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Pages;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.StepDefinitions
{
    [Binding]
    public class ValidateMovementStepDefinitions
    {
        private readonly MovementPage _page;

        public ValidateMovementStepDefinitions(MovementPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
        }

        [Given("{string} is selected")]
        public async Task GivenIsSelected(string thunderbird)
        {
            await _page.GotoAsync();
            await _page.SelectThunderbirdAsync(thunderbird);
        }

        [Given("{string} is selected as the start location")]
        public async Task GivenIsSelectedAsTheStartLocation(string location)
        {
            await _page.SelectStartLocationAsync(location);
        }

        [Given("{string} is selected as the destination")]
        public async Task GivenIsSelectedAsTheDestination(string location)
        {
            await _page.SelectDestinationAsync(location);
        }

        [When("the movement is validated")]
        public async Task WhenTheMovementIsValidated()
        {
            await _page.ClickCalculateButton();
        }

        [Then("the validation success result should be displayed")]
        public async Task ThenTheValidationSuccessResultShouldBeDisplayed()
        {
            await _page.AssertValidationSuccessDisplayed();
        }

        [Then("the validation failure result should be displayed")]
        public async Task ThenTheValidationFailureResultShouldBeDisplayed()
        {
            await _page.AssertValidationFailureDisplayed();
        }
    }
}
