using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal sealed class ReferenceFabCardCatalogLookup : IFabCardCatalogLookup
    {
        private readonly IFabCardDefinitionCatalog _catalog;

        public ReferenceFabCardCatalogLookup(IFabCardDefinitionCatalog catalog)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        }

        public bool Exists(CardCode cardCode)
        {
            return _catalog.Exists(cardCode);
        }
    }
}
