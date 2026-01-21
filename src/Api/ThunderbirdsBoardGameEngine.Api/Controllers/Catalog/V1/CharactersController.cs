using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThunderbirdsBoardGameEngine.Api.Mappers.Catalog.V1;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.Api.Controllers.Catalog.V1
{
    [Route("api/catalog/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [AllowAnonymous]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterDefinitionService _service;

        public CharactersController(ICharacterDefinitionService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CharacterDto>), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var characters = _service.GetAll();

            return Ok(characters.ToDto());
        }
    }
}
