using System.Collections.Frozen;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs
{
    internal sealed class EventCardDefinitionCatalog : IEventCardDefinitionCatalog
    {
        private readonly FrozenDictionary<CardCode, ReferenceEventCardDefinition> _byCode;

        public EventCardDefinitionCatalog(ReferenceDataSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            _byCode = snapshot.EventCardDefinitions
                .ToDictionary(c => c.Code)
                .ToFrozenDictionary();
        }

        public bool Exists(CardCode cardCode)
        {
            return _byCode.ContainsKey(cardCode);
        }
    }
}
