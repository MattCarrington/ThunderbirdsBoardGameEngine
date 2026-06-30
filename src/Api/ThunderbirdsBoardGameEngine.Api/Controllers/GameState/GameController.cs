using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;
using ThunderbirdsBoardGameEngine.GameState.Application.GetGame;
using ThunderbirdsBoardGameEngine.GameState.Contracts.V1;
using ThunderbirdsBoardGameEngine.GameState.Domain;

namespace ThunderbirdsBoardGameEngine.Api.Controllers.GameState
{
    [Route("api/games/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [AllowAnonymous]
    public class GameController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{gameId}")]
        public async Task<IActionResult> GetGameState([FromRoute] Guid gameId, CancellationToken cancellationToken)
        {
            var query = new GetGameSessionQuery(gameId);

            var response = await _mediator.Send(query, cancellationToken);

            var result = Map(response.GameSession);

            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGameState(CancellationToken cancellationToken)
        {
            var command = new CreateGameSessionCommand();

            var response = await _mediator.Send(command, cancellationToken);

            var result = Map(response.GameSession);

            return Ok(result);
        }

        private GameSessionDto Map(GameSession response)
        {
            var thunderbirdLocations = new List<ThunderbirdLocationDto>();

            foreach (var thunderbirdLocation in response.VehicleDefinitions)
            {
                thunderbirdLocations.Add(new ThunderbirdLocationDto
                {
                    ThunderbirdCode = thunderbirdLocation.Key.Value,
                    LocationCode = thunderbirdLocation.Value.Value
                });
            }

            return new GameSessionDto
            {
                GameId = response.GameId,
                ThunderbirdLocation = thunderbirdLocations
            };
        }
    }
}
