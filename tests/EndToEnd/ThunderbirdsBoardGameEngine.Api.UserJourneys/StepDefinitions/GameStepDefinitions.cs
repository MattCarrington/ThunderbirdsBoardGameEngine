using Reqnroll;
using System;
using ThunderbirdsBoardGameEngine.Api.UserJourneys.Supports;
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

            Assert.True(result.Success);

            var game = Assert.IsType<GameStateResponseDto>(result.Data);

            _context.GameId = game.GameId;
            _context.CurrentGame = game;
        }

        [When("the player moves {string} to {string}")]
        public async Task WhenThePlayerMovesToAsync(string thunderbirdMachine, string destination)
        {
            var dto = new MoveThunderbirdMachineRequestDto
            {
                Destination = Slugify(destination)
            };

            var result = await _client.MoveThunderbirdMachineAsync(
                _context.GameId,
                Slugify(thunderbirdMachine),
                dto,
                TestContext.Current.CancellationToken);

            Assert.True(result.Success);

            var game = Assert.IsType<GameStateResponseDto>(result.Data);

            _context.CurrentGame = game;
        }

        [Then("{string} should be at {string}")]
        public void ThenShouldBeAt(string thunderbirdMachine, string location)
        {
            var machine = Assert.Single(_context.CurrentGame!.ThunderbirdMachines, machine => machine.ThunderbirdCode == Slugify(thunderbirdMachine));

            Assert.Equal(Slugify(location), machine.LocationCode);
        }

        private string Slugify(string input)
        {
            return input.ToLowerInvariant().Replace(" ", "-");
        }
    }
}
