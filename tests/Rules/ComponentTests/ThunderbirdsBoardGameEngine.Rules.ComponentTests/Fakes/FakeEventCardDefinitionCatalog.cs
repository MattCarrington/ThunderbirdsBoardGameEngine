using System.Collections.Frozen;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes
{
    public class FakeEventCardDefinitionCatalog : IEventCardDefinitionCatalog
    {
        private readonly FrozenDictionary<CardCode, ReferenceEventCardDefinition> _eventCards;

        public FakeEventCardDefinitionCatalog(params ReferenceEventCardDefinition[] eventCards)
        {
            _eventCards = eventCards.ToFrozenDictionary(eventCard => eventCard.Code);
        }

        public bool Exists(CardCode cardCode)
        {
            return _eventCards.ContainsKey(cardCode);
        }
    }
}
