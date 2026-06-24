using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal sealed class ReferenceEventCardCatalogLookup : IEventCardCatalogLookup
    {
        private readonly IEventCardDefinitionCatalog _catalog;

        public ReferenceEventCardCatalogLookup(IEventCardDefinitionCatalog catalog)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        }

        public bool Exists(CardCode cardCode)
        {
            return _catalog.Exists(cardCode);
        }
    }
}
