using System.Collections.Frozen;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs
{
    internal sealed class FabCardDefinitionCatalog : IFabCardDefinitionCatalog
    {
        private readonly FrozenDictionary<CardCode, ReferenceFabCardDefinition> _byCode;

        public FabCardDefinitionCatalog(ReferenceDataSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            _byCode = snapshot.FabCardDefinitions
                .ToDictionary(c => c.Code)
                .ToFrozenDictionary();
        }

        public bool Exists(CardCode cardCode)
        {
            return _byCode.ContainsKey(cardCode);
        }
    }
}
