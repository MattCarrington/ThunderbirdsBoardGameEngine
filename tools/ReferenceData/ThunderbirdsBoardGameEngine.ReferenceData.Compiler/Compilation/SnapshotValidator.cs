using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using System.Linq;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation
{
    public sealed class SnapshotValidator
    {
        public void Validate(ReferenceDataSnapshot snapshot)
        {
            EnsureLocationDefinitionsValid(snapshot);
            EnsureCharacterDefinitionsValid(snapshot);
            EnsureThunderbirdDefinitionsValid(snapshot);
            EnsurePodVehicleDefinitionsValid(snapshot);
            EnsureDisasterDefinitionsValid(snapshot);

            var locations = new HashSet<LocationCode>(snapshot.LocationDefinitions.Select(l => l.Code));
            EnsureDisasterDefinitionLocationsValid(snapshot.DisasterDefinitions, locations);

            var validDisasterBonusKeys = EnsureEntityCodesUniqueAcrossTypes(snapshot);
            EnsureBonusKeysValid(snapshot.DisasterDefinitions, validDisasterBonusKeys);
        }

        private static void EnsureLocationDefinitionsValid(ReferenceDataSnapshot snapshot)
        {
            EnsureUniqueValues(snapshot.LocationDefinitions, l => l.Code.Value, "location codes");
            EnsureUniqueValues(snapshot.LocationDefinitions, l => l.DisplayName, "location names");
        }

        private static void EnsureCharacterDefinitionsValid(ReferenceDataSnapshot snapshot)
        {
            EnsureUniqueValues(snapshot.CharacterDefinitions, c => c.Code.Value, "character codes");
            EnsureUniqueValues(snapshot.CharacterDefinitions, c => c.DisplayName, "character names");
        }

        private static void EnsureDisasterDefinitionsValid(ReferenceDataSnapshot snapshot)
        {
            EnsureUniqueValues(snapshot.DisasterDefinitions, d => d.Code.Value, "disaster codes");
            EnsureUniqueValues(snapshot.DisasterDefinitions, d => d.DisplayName, "disaster names");

            var locations = new HashSet<LocationCode>(snapshot.LocationDefinitions.Select(l => l.Code));

            EnsureDisasterDefinitionLocationsValid(snapshot.DisasterDefinitions, locations);
        }

        private static void EnsureThunderbirdDefinitionsValid(ReferenceDataSnapshot snapshot)
        {
            EnsureUniqueValues(snapshot.ThunderbirdDefinitions, t => t.Code.Value, "thunderbird codes");
            EnsureUniqueValues(snapshot.ThunderbirdDefinitions, t => t.DisplayName, "thunderbird names");
        }

        private static void EnsurePodVehicleDefinitionsValid(ReferenceDataSnapshot snapshot)
        {
            EnsureUniqueValues(snapshot.PodVehicleDefinitions, v => v.Code.Value, "pod vehicle codes");
            EnsureUniqueValues(snapshot.PodVehicleDefinitions, v => v.DisplayName, "pod vehicle names");
        }

        private static void EnsureUniqueValues<T>(
            IReadOnlyList<T> items,
            Func<T, string> selector,
            string entityDescription)
        {
            var duplicates = items
                .GroupBy(selector)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count != 0)
            {
                throw new ReferenceDataCompilationException($"Duplicate {entityDescription} found: {string.Join(", ", duplicates)}");
            }
        }

        private static void EnsureDisasterDefinitionLocationsValid(
            IReadOnlyList<ReferenceDisasterDefinition> disasterDefinitions,
            HashSet<LocationCode> validLocationCodes)
        {
            EnsureDisasterDefinitionsHaveValidLocations(disasterDefinitions, validLocationCodes);
            EnsureDisasterBonusesHaveValidLocations(disasterDefinitions, validLocationCodes);
        }

        private static void EnsureDisasterBonusesHaveValidLocations(
            IReadOnlyList<ReferenceDisasterDefinition> disasterDefinitions,
            HashSet<LocationCode> validLocationCodes)
        {
            var invalidBonuses = new List<string>();

            foreach (var disaster in disasterDefinitions)
            {
                foreach (var bonus in disaster.Bonuses)
                {
                    if (bonus.Location is LocationCode bonusLocation && !validLocationCodes.Contains(bonusLocation))
                    {
                        invalidBonuses.Add($"{disaster.DisplayName} - Bonus '{bonus.Key.Value}' (location: {bonusLocation.Value})");
                    }
                }
            }

            if (invalidBonuses.Count != 0)
            {
                throw new ReferenceDataCompilationException($"Bonuses reference non-existent locations: {string.Join(", ", invalidBonuses)}");
            }
        }

        private static void EnsureDisasterDefinitionsHaveValidLocations(IReadOnlyList<ReferenceDisasterDefinition> disasterDefinitions, HashSet<LocationCode> validLocationCodes)
        {
            var invalidLocationCodes = disasterDefinitions
                .Select(d => d.Location)
                .Where(l => !validLocationCodes.Contains(l))
                .Select(l => l.Value)
                .ToList();

            if (invalidLocationCodes.Count != 0)
            {
                throw new ReferenceDataCompilationException($"Invalid location codes found in disaster definitions: {string.Join(", ", invalidLocationCodes)}");
            }
        }

        private static HashSet<string> EnsureEntityCodesUniqueAcrossTypes(ReferenceDataSnapshot snapshot)
        {
            var seenCodes = new HashSet<string>();
            var duplicates = new List<(string Code, string Type)>();

            // Check characters
            foreach (var character in snapshot.CharacterDefinitions)
            {
                if (!seenCodes.Add(character.Code.Value))
                {
                    duplicates.Add((character.Code.Value, "Character"));
                }
            }

            // Check thunderbirds
            foreach (var thunderbird in snapshot.ThunderbirdDefinitions)
            {
                if (!seenCodes.Add(thunderbird.Code.Value))
                {
                    duplicates.Add((thunderbird.Code.Value, "Thunderbird"));
                }
            }

            // Check pod vehicles
            foreach (var podVehicle in snapshot.PodVehicleDefinitions)
            {
                if (!seenCodes.Add(podVehicle.Code.Value))
                {
                    duplicates.Add((podVehicle.Code.Value, "Pod Vehicle"));
                }
            }

            if (duplicates.Count != 0)
            {
                var duplicateMessages = duplicates
                    .Select(d => $"'{d.Code}' ({d.Type})")
                    .ToList();

                throw new ReferenceDataCompilationException(
                    $"Entity codes must be unique across all types. Duplicates: {string.Join(", ", duplicateMessages)}");
            }

            return seenCodes;
        }

        private static void EnsureBonusKeysValid(
            IReadOnlyList<ReferenceDisasterDefinition> disasterDefinitions,
            HashSet<string> validEntityCodes)
        {
            var invalidBonuses = new List<string>();

            foreach (var disaster in disasterDefinitions)
            {
                invalidBonuses.AddRange(disaster.Bonuses
                    .Where(bonus => !validEntityCodes
                    .Contains(bonus.Key.Value))
                    .Select(bonus => $"{disaster.DisplayName} - Unknown bonus key '{bonus.Key.Value}'"));
            }

            if (invalidBonuses.Count != 0)
            {
                throw new ReferenceDataCompilationException(
                    $"Bonuses reference non-existent entities: {string.Join(", ", invalidBonuses)}");
            }
        }
    }
}
