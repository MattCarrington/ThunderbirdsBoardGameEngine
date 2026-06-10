using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Services
{
    public class MovementLocationOptionsService : IMovementLocationOptionsService
    {
        private readonly ILocationDefinitionCatalog _catalog;

        public MovementLocationOptionsService(ILocationDefinitionCatalog catalog)
        {
            _catalog = catalog;
        }

        public IReadOnlyList<MovementLocationOptions> GetAll()
        {
            return _catalog.GetAll()
                .Select(l => new MovementLocationOptions(l.Code.Value, l.DisplayName)).ToList();
        }
    }
}
