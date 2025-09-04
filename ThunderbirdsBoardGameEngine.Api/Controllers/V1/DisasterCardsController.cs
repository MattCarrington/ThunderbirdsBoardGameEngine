using Microsoft.AspNetCore.Mvc;
using ThunderbirdsBoardGameEngine.Api.Mappers.V1;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;

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
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var disasterCards = await _service.GetAllAsync(cancellationToken);

            return Ok(disasterCards.ToDto());
        }
    }
}
