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
            _keys = BuildBonusKeyDictionary(snapshot);
        }

        public DisasterBonusKeyDefinition GetByCode(DisasterBonusKey key)
        {
            if (!_keys.TryGetValue(key, out var definition))
            {
                throw new KeyNotFoundException($"No disaster bonus key definition found for key '{key}'.");
            }

            return definition;
        }

        private static FrozenDictionary<DisasterBonusKey, DisasterBonusKeyDefinition> BuildBonusKeyDictionary(
            ReferenceDataSnapshot snapshot)
        {
            var keys = new List<DisasterBonusKeyDefinition>();

            // Characters
            keys.AddRange(snapshot.CharacterDefinitions.Select(c =>
                new DisasterBonusKeyDefinition(new DisasterBonusKey(c.Code.Value), c.DisplayName)));

            // Thunderbirds
            keys.AddRange(snapshot.ThunderbirdDefinitions.Select(t =>
                new DisasterBonusKeyDefinition(new DisasterBonusKey(t.Code.Value), t.DisplayName)));

            // Pod Vehicles
            keys.AddRange(snapshot.PodVehicleDefinitions.Select(v =>
                new DisasterBonusKeyDefinition(new DisasterBonusKey(v.Code.Value), v.DisplayName)));

            return keys.ToFrozenDictionary(k => k.Key);
        }
    }
}
