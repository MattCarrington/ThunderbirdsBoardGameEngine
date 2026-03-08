using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation
{
    public sealed class SnapshotValidator
    {
        public void Validate(ReferenceDataSnapshot snapshot)
        {
            EnsureUniqueDisasterCodes(snapshot);
            EnsureUniqueDisasterNames(snapshot);
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
    }
}
