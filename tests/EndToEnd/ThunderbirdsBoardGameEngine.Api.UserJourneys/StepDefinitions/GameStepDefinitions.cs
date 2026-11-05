using Reqnroll;
using Reqnroll.CommonModels;
using System;
using ThunderbirdsBoardGameEngine.Api.UserJourneys.Support;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.GameState.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.TransferCharacters;
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

        [When("the player transfers {string} to {string}")]
        public async Task WhenThePlayerTransfersTo(string characterCode, string destinationThunderbirdCode)
        {
            var result = await TransferCharactersAsync(characterCode, destinationThunderbirdCode);

            UpdateGameContext(result);
        }

        [When("the player swaps {string} in {string} with {string} in {string}")]
        public async Task WhenThePlayerSwapsInWithInAsync(string characterOneCode, string firstThunderbirdCode, string characterTwoCode, string secondThunderbirdCode)
        {
            var dto = new TransferCharactersRequestDto
            {
                Assignments =
                [
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = Slugify(characterOneCode),
                        DestinationThunderbirdCode = Slugify(secondThunderbirdCode)
                    },
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = Slugify(characterTwoCode),
                        DestinationThunderbirdCode = Slugify(firstThunderbirdCode)
                    }
                ]
            };

            var result = await _client.TransferCharactersAsync(_context.GameId, dto, TestContext.Current.CancellationToken);

            UpdateGameContext(result);
        }

        [When("the player attempts to transfer {string} to {string}")]
        public async Task WhenThePlayerAttemptsToTransferToAsync(string characterCode, string destinationThunderbirdCode)
        {
            var result = await TransferCharactersAsync(characterCode, destinationThunderbirdCode);

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

        [Then("{string} should be occupying {string}")]
        public async Task ThenShouldBeOccupyingAsync(string character, string thunderbirdCode)
        {
            var result = await _client.GetGameStateAsync(_context.GameId, TestContext.Current.CancellationToken);

            UpdateGameContext(result);

            var machine = Assert.Single(_context.CurrentGame!.ThunderbirdMachines, machine => machine.ThunderbirdCode == Slugify(thunderbirdCode));

            Assert.Contains(Slugify(character), machine.OccupantCharacterCodes);
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

        [Then("an error should be returned indicating that the transfer is invalid")]
        public void ThenAnErrorShouldBeReturnedIndicatingThatTheTransferIsInvalid()
        {
            Assert.False(_context.LastApiResult!.Success);
        }


        private static string Slugify(string input)
        {
            return input.ToLowerInvariant().Replace(" ", "-");
        }

        private void UpdateGameContext(ApiResult<GameStateResponseDto> result)
        {
            Assert.True(
                result.Success,
                $"Request failed with {(int)result.StatusCode} {result.StatusCode}: {result.ErrorMessage}");

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

        private async Task<ApiResult<GameStateResponseDto>> TransferCharactersAsync(string characterCode, string destinationThunderbirdCode)
        {
            var dto = new TransferCharactersRequestDto
            {
                Assignments =
                [
                    new CharacterTransferAssignmentDto
                    {
                        CharacterCode = Slugify(characterCode),
                        DestinationThunderbirdCode = Slugify(destinationThunderbirdCode)
                    }
                ]
            };

            return await _client.TransferCharactersAsync(_context.GameId, dto, TestContext.Current.CancellationToken);
        }
    }
}
