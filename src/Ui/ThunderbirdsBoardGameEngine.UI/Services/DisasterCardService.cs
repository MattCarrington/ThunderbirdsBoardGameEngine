using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Mappers;

namespace ThunderbirdsBoardGameEngine.UI.Services
{
    public class DisasterCardService : IDisasterCardService
    {
        private readonly IDisasterDefinitionCatalog _catalog;
        private readonly DisasterCardMapper _mapper;

        public DisasterCardService(IDisasterDefinitionCatalog catalog, DisasterCardMapper mapper)
        {
            _catalog = catalog;
            _mapper = mapper;
        }

        public DisasterCardViewModel GetByCode(string code)
        {
            var disaster = _catalog.GetByCode(new CardCode(code));

            return _mapper.Map(disaster);
        }

        public IReadOnlyList<DisasterCardViewModel> GetAll()
        {
            return _catalog.GetAll().Select(_mapper.Map).ToList();
        }
    }
}
