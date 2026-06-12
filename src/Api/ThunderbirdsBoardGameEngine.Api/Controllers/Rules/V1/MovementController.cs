using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ThunderbirdsBoardGameEngine.Api.Mappers.Rules.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.AccessibleLocations;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.AccessibleLocations.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;

namespace ThunderbirdsBoardGameEngine.Api.Controllers.Rules.V1
{
    [Route("api/rules/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [AllowAnonymous]
    [EnableRateLimiting("public-api")]
    public class MovementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MovementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{thunderbirdCode}/validate")]
        public async Task<IActionResult> ValidateMovement([FromRoute] string thunderbirdCode, [FromBody] ValidateMovementRequestDto request, CancellationToken cancellationToken)
        {
            var query = request.ToQuery(thunderbirdCode);

            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response.ToDto());
        }

        [HttpGet("{thunderbirdCode}/accessible-locations")]
        public async Task<IActionResult> GetAccessibleLocations([FromRoute] string thunderbirdCode, CancellationToken cancellationToken)
        {
            var query = new FindAccessibleLocationsQuery(new ThunderbirdCode(thunderbirdCode));

            var result = await _mediator.Send(query, cancellationToken);

            var response = new AccessibleLocationsResponseDto
            {
                AccessibleLocations = result.AccessibleLocations.Select(location => location.Value).ToList()
            };

            return Ok(response);
        }
    }
}
