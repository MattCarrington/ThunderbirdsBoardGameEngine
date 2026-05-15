using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ThunderbirdsBoardGameEngine.Api.Mappers.Rules.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;

namespace ThunderbirdsBoardGameEngine.Api.Controllers.Rules.V1
{
    [Route("api/rules/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [AllowAnonymous]
    [EnableRateLimiting("public-api")]
    public class RescueController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RescueController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{disasterCardCode}/target")]
        public async Task<IActionResult> CalculateRescueTarget([FromRoute] string disasterCardCode, [FromBody] CalculateRescueTargetRequestDto request, CancellationToken cancellationToken)
        {
            var query = request.ToQuery(disasterCardCode);

            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response.ToDto());
        }
    }
}
