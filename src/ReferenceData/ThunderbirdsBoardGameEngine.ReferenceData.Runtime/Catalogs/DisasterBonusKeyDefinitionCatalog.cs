using System.Collections.Frozen;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Models;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs
{
    internal class DisasterBonusKeyDefinitionCatalog : IDisasterBonusKeyDefinitionCatalog
    {
        private readonly FrozenDictionary<DisasterBonusKey, DisasterBonusKeyDefinition> _keys;

        public DisasterBonusKeyDefinitionCatalog(ReferenceDataSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            _keys = snapshot.CharacterDefinitions.Select(c => new DisasterBonusKeyDefinition(new DisasterBonusKey(c.Code.Value), c.DisplayName))
                .Concat(snapshot.ThunderbirdDefinitions.Select(t => new DisasterBonusKeyDefinition(new DisasterBonusKey(t.Code.Value), t.DisplayName)))
                .Concat(snapshot.PodVehicleDefinitions.Select(v => new DisasterBonusKeyDefinition(new DisasterBonusKey(v.Code.Value), v.DisplayName)))
                .ToFrozenDictionary(k => k.Key);
        }

        public DisasterBonusKeyDefinition GetByCode(DisasterBonusKey key)
        {
            if (!_keys.TryGetValue(key, out var definition))
            {
                throw new KeyNotFoundException($"No disaster bonus key definition found for key '{key}'.");
            }

            return definition;
        }
    }
}
