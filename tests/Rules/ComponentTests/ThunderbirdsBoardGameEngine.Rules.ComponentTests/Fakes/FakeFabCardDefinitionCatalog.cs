using System.Collections.Frozen;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes
{
    public class FakeFabCardDefinitionCatalog : IFabCardDefinitionCatalog
    {
        private readonly FrozenDictionary<CardCode, ReferenceFabCardDefinition> _fabCards;

        public FakeFabCardDefinitionCatalog(params ReferenceFabCardDefinition[] fabCards)
        {
            _fabCards = fabCards.ToFrozenDictionary(fabCard => fabCard.Code);
        }

        public bool Exists(CardCode cardCode)
        {
            return _fabCards.ContainsKey(cardCode);
        }
    }
}
