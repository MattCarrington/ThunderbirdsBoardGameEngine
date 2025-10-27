using Microsoft.AspNetCore.Mvc;
using ThunderbirdsBoardGameEngine.Api.Mappers.V1;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.Api.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class DisasterCardsController : ControllerBase
    {
        private readonly IDisasterCardService _service;

        public DisasterCardsController(IDisasterCardService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DisasterCardDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        public IActionResult Get(CancellationToken _)
        {
            var cards = _service.GetAll();

            return Ok(cards.ToDto());
        }
    }
}
