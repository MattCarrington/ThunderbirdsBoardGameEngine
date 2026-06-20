using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Mappers;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Services
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

        public IReadOnlyList<DisasterCardSummaryViewModel> GetAll()
        {
            var disasters = _catalog.GetAll();

            var summaries = disasters.Select(d => new DisasterCardSummaryViewModel(d.Code.Value, d.DisplayName)).ToList();

            return summaries.OrderBy(c => c.DisplayName ?? string.Empty, StringComparer.OrdinalIgnoreCase).ToList();
        }
    }
}
