using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators
{
    /// <summary>
    /// Validates the disaster bonus system's polymorphic asset references.
    /// Ensures:
    /// 1. Asset codes (Characters, Thunderbirds, Pod Vehicles) are unique across types
    ///    to prevent ambiguous references in disaster bonuses.
    /// 2. All disaster bonus keys reference valid assets from the union.
    /// </summary>
    public sealed class DisasterBonusSystemValidator : ISnapshotValidator
    {
        public void Validate(ReferenceDataSnapshot snapshot)
        {
            // Asset codes must be unique across types for unambiguous polymorphic references
            var validAssetCodes = EnsureAssetCodesUniqueAcrossTypes(snapshot);

            // Disaster bonus keys must reference valid assets
            EnsureBonusKeysReferenceValidAssets(snapshot.DisasterDefinitions, validAssetCodes);
            EnsureBonusLocationDifferentToDisaster(snapshot);
        }

        private static HashSet<string> EnsureAssetCodesUniqueAcrossTypes(ReferenceDataSnapshot snapshot)
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
                    $"Asset codes must be unique across types to prevent ambiguous bonus references. Duplicates: {string.Join(", ", duplicateMessages)}");
            }

            return seenCodes;
        }

        private static void EnsureBonusKeysReferenceValidAssets(
            IReadOnlyList<ReferenceDisasterDefinition> disasterDefinitions,
            HashSet<string> validAssetCodes)
        {
            var invalidBonuses = new List<string>();

            foreach (var disaster in disasterDefinitions)
            {
                invalidBonuses.AddRange(disaster.Bonuses
                    .Where(bonus => !validAssetCodes.Contains(bonus.Key.Value))
                    .Select(bonus => $"{disaster.DisplayName} - Unknown bonus key '{bonus.Key.Value}'"));
            }

            if (invalidBonuses.Count != 0)
            {
                throw new ReferenceDataCompilationException(
                    $"Bonuses reference non-existent assets: {string.Join(", ", invalidBonuses)}");
            }
        }

        private static void EnsureBonusLocationDifferentToDisaster(ReferenceDataSnapshot snapshot)
        {
            foreach (var disaster in snapshot.DisasterDefinitions)
            {
                foreach (var bonus in disaster.Bonuses)
                {
                    if (bonus.Location.HasValue && bonus.Location.Value == disaster.Location)
                    {
                        throw new ReferenceDataCompilationException(
                            $"Bonus '{bonus.Key.Value}' in disaster '{disaster.DisplayName}' cannot have the same location as the disaster itself.");
                    }
                }
            }
        }
    }
}