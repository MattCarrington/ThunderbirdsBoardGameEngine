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
        public async Task<IActionResult> Get()
        {
            var disasterCards = await _service.GetAllAsync();

            return Ok(disasterCards.ToDto());

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var disasterCard = await _service.GetByIdAsync(id);

            if (disasterCard is null)
            {
                return NotFound($"Disaster card with ID {id} not found.");
            }

            return Ok(disasterCard.ToDto());
        }
    }
}
