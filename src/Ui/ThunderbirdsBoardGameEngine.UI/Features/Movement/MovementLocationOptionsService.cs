using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement
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
