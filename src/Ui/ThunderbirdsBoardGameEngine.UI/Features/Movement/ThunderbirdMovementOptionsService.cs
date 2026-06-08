using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement
{
    public class ThunderbirdMovementOptionsService : IThunderbirdMovementOptionsService
    {
        private readonly IThunderbirdDefinitionCatalog _catalog;
        public ThunderbirdMovementOptionsService(IThunderbirdDefinitionCatalog catalog)
        {
            _catalog = catalog;
        }

        public IReadOnlyList<ThunderbirdMovementOptions> GetAllMobileVehicles()
        {
            return _catalog.GetAll()
                .Where(t => t.TopSpeed > 0)
                .Select(t => new ThunderbirdMovementOptions(t.Code.Value, t.DisplayName)).ToList();
        }
    }
}
