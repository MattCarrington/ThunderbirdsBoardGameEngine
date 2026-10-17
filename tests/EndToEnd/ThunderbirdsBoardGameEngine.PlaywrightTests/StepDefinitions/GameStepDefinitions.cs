using System;
using Reqnroll;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Pages;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Services;
using ThunderbirdsBoardGameEngine.PlaywrightTests.Support;

namespace ThunderbirdsBoardGameEngine.PlaywrightTests.StepDefinitions
{
    [Binding]
    public class GameStepDefinitions
    {
        private readonly HomePage _homePage;
        private readonly GameDashboardPage _gameDashboardPage;
        private readonly GamePrerequisiteService _gamePrerequisiteService;
        private readonly PlaywrightTestContext _textContext;

        public GameStepDefinitions(HomePage homePage, GameDashboardPage gameDashboardPage, GamePrerequisiteService gamePrerequisiteService, PlaywrightTestContext textContext)
        {
            _homePage = homePage;
            _gameDashboardPage = gameDashboardPage;
            _gamePrerequisiteService = gamePrerequisiteService;
            _textContext = textContext;
        }

        [Given("a game has been created")]
        public async Task GivenAGameHasBeenCreatedAsync()
        {
            _textContext.GameId = await _gamePrerequisiteService.CreateGameAsync();
        }

        [Given("the companion application is open")]
        public async Task GivenTheCompanionApplicationIsOpen()
        {
            await _homePage.GoToAsync();
        }

        [When("the player starts a new game")]
        public async Task WhenThePlayerStartsANewGame()
        {
            await _homePage.ClickNewGameAsync();
        }

        [Given("the player has opened its dashboard")]
        public async Task GivenThePlayerHasOpenedItsDashboard()
        {
            await _homePage.GoToAsync();
            await _homePage.ResumeGameAsync(_textContext.RequireGameId());
        }

        [When("the player resumes the game using its identifier")]
        public async Task WhenThePlayerResumesTheGameUsingItsIdentifier()
        {
            await _homePage.ResumeGameAsync(_textContext.RequireGameId());
        }

        [When("reloads the dashboard")]
        public async Task WhenReloadsTheDashboardAsync()
        {
            await _homePage.GoToAsync();
            await _homePage.ResumeGameAsync(_textContext.RequireGameId());
        }

        [When("the player moves {string} to {string}")]
        public async Task WhenThePlayerMovesTo(string thunderbird, string location)
        {
            await _gameDashboardPage.MoveThunderbirdAsync(thunderbird, location);
        }

        [Then("the game dashboard should be displayed")]
        public async Task ThenTheGameDashboardShouldBeDisplayed()
        {
            await _gameDashboardPage.AssertPageDisplayedAsync();
        }

        [Then("{string} should be in {string}")]
        public async Task ThenShouldBeIn(string thunderbird, string location)
        {
            await _gameDashboardPage.AssertThunderbirdLocationAsync(thunderbird, location);
        }

    }
}
