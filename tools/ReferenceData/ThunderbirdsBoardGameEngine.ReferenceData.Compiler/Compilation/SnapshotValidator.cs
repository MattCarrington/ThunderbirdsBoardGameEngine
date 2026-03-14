using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation
{
    public sealed class SnapshotValidator
    {
        public void Validate(ReferenceDataSnapshot snapshot)
        {
            EnsureLocationDefinitionsValid(snapshot);
            EnsureCharacterDefinitionsValid(snapshot);
            EnsureDisasterDefinitionsValid(snapshot);
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
    }
}
