using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation.Validators
{
    /// <summary>
    /// Validates that entity codes and display names are unique within each entity type.
    /// Universal validation applied to all entity collections.
    /// </summary>
    public sealed class EntityUniquenessValidator : ISnapshotValidator
    {
        public void Validate(ReferenceDataSnapshot snapshot)
        {
            ValidateEntitySet(snapshot.LocationDefinitions, l => l.Code.Value, l => l.DisplayName, "location");
            ValidateEntitySet(snapshot.CharacterDefinitions, c => c.Code.Value, c => c.DisplayName, "character");
            ValidateEntitySet(snapshot.ThunderbirdDefinitions, t => t.Code.Value, t => t.DisplayName, "thunderbird");
            ValidateEntitySet(snapshot.PodVehicleDefinitions, v => v.Code.Value, v => v.DisplayName, "pod vehicle");
            ValidateEntitySet(snapshot.DisasterDefinitions, d => d.Code.Value, d => d.DisplayName, "disaster");
        }

        private static void ValidateEntitySet<T>(
            IReadOnlyList<T> items,
            Func<T, string> codeSelector,
            Func<T, string> nameSelector,
            string entityTypeName)
        {
            EnsureUniqueValues(items, codeSelector, $"{entityTypeName} codes");
            EnsureUniqueValues(items, nameSelector, $"{entityTypeName} names");
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
                throw new ReferenceDataCompilationException(
                    $"Duplicate {entityDescription} found: {string.Join(", ", duplicates)}");
            }
        }
    }
}