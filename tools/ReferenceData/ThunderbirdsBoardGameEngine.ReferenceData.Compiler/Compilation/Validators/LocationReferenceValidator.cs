using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation.Validators
{
    /// <summary>
    /// Validates that location references point to valid location definitions.
    /// Reusable foreign key validation pattern.
    /// </summary>
    public sealed class LocationReferenceValidator : ISnapshotValidator
    {
        public void Validate(ReferenceDataSnapshot snapshot)
        {
            var validLocationCodes = new HashSet<LocationCode>(
                snapshot.LocationDefinitions.Select(l => l.Code));

            ValidateDisasterLocations(snapshot.DisasterDefinitions, validLocationCodes);
            ValidateBonusLocations(snapshot.DisasterDefinitions, validLocationCodes);
        }

        private static void ValidateDisasterLocations(
            IReadOnlyList<ReferenceDisasterDefinition> disasterDefinitions,
            HashSet<LocationCode> validLocationCodes)
        {
            var invalidLocationCodes = disasterDefinitions
                .Select(d => d.Location)
                .Where(l => !validLocationCodes.Contains(l))
                .Select(l => l.Value)
                .ToList();

            if (invalidLocationCodes.Count != 0)
            {
                throw new ReferenceDataCompilationException(
                    $"Invalid location codes found in disaster definitions: {string.Join(", ", invalidLocationCodes)}");
            }
        }

        private static void ValidateBonusLocations(
            IReadOnlyList<ReferenceDisasterDefinition> disasterDefinitions,
            HashSet<LocationCode> validLocationCodes)
        {
            var invalidBonuses = new List<string>();

            foreach (var disaster in disasterDefinitions)
            {
                foreach (var bonus in disaster.Bonuses)
                {
                    if (bonus.Location is LocationCode bonusLocation &&
                        !validLocationCodes.Contains(bonusLocation))
                    {
                        invalidBonuses.Add(
                            $"{disaster.DisplayName} - Bonus '{bonus.Key.Value}' (location: {bonusLocation.Value})");
                    }
                }
            }

            if (invalidBonuses.Count != 0)
            {
                throw new ReferenceDataCompilationException(
                    $"Bonuses reference non-existent locations: {string.Join(", ", invalidBonuses)}");
            }
        }
    }
}