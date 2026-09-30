using Reqnroll;
using Reqnroll.CommonModels;
using System;
using ThunderbirdsBoardGameEngine.Api.UserJourneys.Support;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.GameState.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UserJourneys.StepDefinitions
{
    [Binding]
    public class GameStepDefinitions
    {
        private readonly IGameClient _client;
        private readonly GameJourneyContext _context;

        public GameStepDefinitions(IGameClient client, GameJourneyContext context)
        {
            _client = client;
            _context = context;
        }


        [Given("a new game has been created")]
        public async Task GivenANewGameHasBeenCreated()
        {
            var result = await _client.CreateGameAsync(TestContext.Current.CancellationToken);

            UpdateGameContext(result);

            _context.ThunderbirdMachineLocations = _context.CurrentGame!.ThunderbirdMachines.ToDictionary(machine => machine.ThunderbirdCode, machine => machine.LocationCode);
        }

        [Given("the player moves {string} to {string}")]
        [When("the player moves {string} to {string}")]
        public async Task WhenThePlayerMovesToAsync(string thunderbirdMachine, string destination)
        {
            var result = await MoveThunderbirdAsync(thunderbirdMachine, destination);

            UpdateGameContext(result);
        }

        [When("the player returns to the game")]
        public async Task WhenThePlayerReturnsToTheGame()
        {
            var result = await _client.GetGameStateAsync(_context.GameId, TestContext.Current.CancellationToken);

            UpdateGameContext(result);
        }

        [When("the player attempts to move {string} to {string}")]
        public async Task WhenThePlayerAttemptsToMoveTo(string thunderbirdCode, string destination)
        {
            var result = await MoveThunderbirdAsync(thunderbirdCode, destination);
            
            _context.LastApiResult = result;
        }

        [Then("{string} should be at {string}")]
        public async Task ThenShouldBeAt(string thunderbirdMachine, string location)
        {
            var result = await _client.GetGameStateAsync(_context.GameId, TestContext.Current.CancellationToken);

            UpdateGameContext(result);

            var machine = Assert.Single(_context.CurrentGame!.ThunderbirdMachines, machine => machine.ThunderbirdCode == Slugify(thunderbirdMachine));

            Assert.Equal(Slugify(location), machine.LocationCode);
        }

        [Then("the game should still be available")]
        public async Task ThenTheGameShouldStillBeAvailable()
        {
            var result = await _client.GetGameStateAsync(_context.GameId, TestContext.Current.CancellationToken);

            UpdateGameContext(result);
        }

        [Then("its Thunderbird positions should match the recorded positions")]
        public void ThenItsThunderbirdPositionsShouldMatchTheRecordedPositions()
        {
            foreach (var (thunderbirdCode, locationCode) in _context.ThunderbirdMachineLocations)
            {
                var machine = Assert.Single(_context.CurrentGame!.ThunderbirdMachines, machine => machine.ThunderbirdCode == thunderbirdCode);
                Assert.Equal(locationCode, machine.LocationCode);
            }
        }

        [Then("an error should be returned indicating that the move is invalid")]
        public void ThenAnErrorShouldBeReturnedIndicatingThatTheMoveIsInvalid()
        {
            Assert.False(_context.LastApiResult!.Success);
        }

        private string Slugify(string input)
        {
            return input.ToLowerInvariant().Replace(" ", "-");
        }

        private void UpdateGameContext(ApiResult<GameStateResponseDto> result)
        {
            Assert.True(result.Success);

            var game = Assert.IsType<GameStateResponseDto>(result.Data);

            _context.GameId = game.GameId;
            _context.CurrentGame = game;
        }

        private async Task<ApiResult<GameStateResponseDto>> MoveThunderbirdAsync(string thunderbirdCode, string destination)
        {
            var dto = new MoveThunderbirdMachineRequestDto
            {
                Destination = Slugify(destination)
            };

            return await _client.MoveThunderbirdMachineAsync(
                _context.GameId,
                Slugify(thunderbirdCode),
                dto,
                TestContext.Current.CancellationToken);
        }
    }
}
