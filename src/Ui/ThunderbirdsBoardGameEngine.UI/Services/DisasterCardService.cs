using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Mappers;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Services
{
    public class DisasterCardService : IDisasterCardService
    {
        private readonly IDisasterDefinitionCatalog _catalog;

        public DisasterCardService(IDisasterDefinitionCatalog catalog)
        {
            _catalog = catalog;
        }

        public DisasterCardViewModel GetByCode(string code)
        {
            return _catalog.GetByCode(new CardCode(code)).ToViewModel();
        }

        public IReadOnlyList<DisasterCardViewModel> GetAll()
        {
            return _catalog.GetAll().Select(d => d.ToViewModel()).ToList();
        }
    }
}
