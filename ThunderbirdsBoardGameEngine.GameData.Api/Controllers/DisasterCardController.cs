using Microsoft.AspNetCore.Mvc;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisasterCardController : ControllerBase
    {
        private readonly IDisasterCardService _service;

        public DisasterCardController(IDisasterCardService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Get()
        {
            var disasterCards = await _service.GetAllAsync();

            return Ok(disasterCards);

        }
    }
}
