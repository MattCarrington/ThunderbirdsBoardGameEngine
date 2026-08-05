using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ThunderbirdsBoardGameEngine.Api.Exceptions;
using ThunderbirdsBoardGameEngine.Api.Mappers.GameState.V1;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;
using ThunderbirdsBoardGameEngine.GameState.Application.GetGame;
using ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Api.Controllers.GameState.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [AllowAnonymous]
    [EnableRateLimiting("public-api")]
    public class GamesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GamesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateNewGameCommand(), cancellationToken);

            return CreatedAtAction(nameof(GetGame), new { gameId = result.GameSession.Id }, result.GameSession.ToDto());
        }

        [HttpGet("{gameId:guid}")]
        public async Task<IActionResult> GetGame([FromRoute] Guid gameId, CancellationToken cancellationToken)
        {
            ValidateGameId(gameId);

            var result = await _mediator.Send(new GetGameQuery(gameId), cancellationToken);

            return Ok(result.GameSession.ToDto());
        }

        [HttpPost("{gameId:guid}/thunderbird-machines/{thunderbirdCode}/move")]
        public async Task<IActionResult> MoveThunderbirdMachine(
            [FromRoute] Guid gameId,
            [FromRoute] string thunderbirdCode,
            [FromBody] MoveThunderbirdMachineRequestDto request,
            CancellationToken cancellationToken)
        {
            ValidateGameId(gameId);

            var command = new MoveThunderbirdCommand(gameId, new ThunderbirdCode(thunderbirdCode), new LocationCode(request.Destination));

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result.GameSession.ToDto());
        }

        private static void ValidateGameId(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                throw new BadRequestException("The provided GUID is empty.");
            }
        }
    }
}
