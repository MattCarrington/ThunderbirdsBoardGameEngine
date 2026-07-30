using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;
using ThunderbirdsBoardGameEngine.GameState.Application.GetGame;
using ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.ThunderbirdMachines.V1;
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
        public async Task<IActionResult> CreateGame()
        {
            var result = await _mediator.Send(new CreateNewGameCommand());

            return Ok(result);
        }

        [HttpGet("{gameId:guid}")]
        public async Task<IActionResult> GetGame([FromRoute] Guid gameId)
        {
            var result = await _mediator.Send(new GetGameQuery(gameId));

            return Ok(result);
        }

        [HttpPost("{gameId:guid}/thunderbird-machines/{thunderbirdCode:string}/move")]
        public async Task<IActionResult> MoveThunderbirdMachine([FromRoute] Guid gameId, [FromRoute] string thunderbirdCode, [FromBody] MoveThunderbirdMachineRequestDto request)
        {
            var command = new MoveThunderbirdCommand(gameId, new ThunderbirdCode(thunderbirdCode), new LocationCode(request.Destination));

            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
