using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation
{
    public sealed class SnapshotValidator
    {
        public void Validate(ReferenceDataSnapshot snapshot)
        {
            EnsureLocationDefinitionsValid(snapshot);
            EnsureDisasterDefinitionsValid(snapshot);
        }

        private static void EnsureUniqueDisasterCodes(ReferenceDataSnapshot snapshot)
        {
            var duplicates = snapshot.DisasterDefinitions
                .GroupBy(d => d.Code.Value)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count != 0)
            {
                throw new ReferenceDataCompilationException($"Duplicate disaster codes found: {string.Join(", ", duplicates)}");
            }
        }

        private static void EnsureUniqueDisasterNames(ReferenceDataSnapshot snapshot)
        {
            var duplicates = snapshot.DisasterDefinitions
                .GroupBy(d => d.DisplayName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count != 0)
            {
                throw new ReferenceDataCompilationException($"Duplicate disaster names found: {string.Join(", ", duplicates)}");
            }
        }

        private static void EnsureUniqueLocationCodes(ReferenceDataSnapshot snapshot)
        {
            var duplicates = snapshot.LocationDefinitions
                .GroupBy(d => d.Code.Value)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count != 0)
            {
                throw new ReferenceDataCompilationException($"Duplicate location codes found: {string.Join(", ", duplicates)}");
            }
        }

        private static void EnsureUniqueLocationNames(ReferenceDataSnapshot snapshot)
        {
            var duplicates = snapshot.LocationDefinitions
                .GroupBy(l => l.DisplayName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count != 0)
            {
                throw new ReferenceDataCompilationException($"Duplicate location names found: {string.Join(", ", duplicates)}");
            }
        }

        private static void EnsureLocationDefinitionsValid(ReferenceDataSnapshot snapshot) 
        { 
            EnsureUniqueValues(snapshot.LocationDefinitions, l => l.Code.Value, "location codes");
            EnsureUniqueValues(snapshot.LocationDefinitions, l => l.DisplayName, "location names");
        }

        private static void EnsureDisasterDefinitionsValid(ReferenceDataSnapshot snapshot) 
        { 
            EnsureUniqueValues(snapshot.DisasterDefinitions, d => d.Code.Value, "disaster codes");
            EnsureUniqueValues(snapshot.DisasterDefinitions, d => d.DisplayName, "disaster names");
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
