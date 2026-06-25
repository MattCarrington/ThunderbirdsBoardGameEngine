using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators
{
    public class MapEdgeValidator : ISnapshotValidator
    {
        public void Validate(ReferenceDataSnapshot snapshot)
        {
            var duplicates = snapshot.MapEdgeDefinitions
                .GroupBy(edge => CreateKey(edge))
                .Where(group => group.Count() > 1)
                .ToList();

            if (duplicates.Count == 0)
            {
                return;
            }

            var duplicateDescriptions = duplicates
                .Select(group => string.Join(", ", group.Select(edge =>
                    $"{edge.Edge1.Value} <-> {edge.Edge2.Value} ({edge.EdgeType})")));

            throw new ReferenceDataCompilationException(
                $"Duplicate map edges found: {string.Join("; ", duplicateDescriptions)}");
        }

        private static string CreateKey(ReferenceMapEdgeDefinition edge)
        {
            var orderedLocations = new[]
            {
                edge.Edge1.Value,
                edge.Edge2.Value
            }
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return $"{orderedLocations[0]}|{orderedLocations[1]}";
        }
    }
}