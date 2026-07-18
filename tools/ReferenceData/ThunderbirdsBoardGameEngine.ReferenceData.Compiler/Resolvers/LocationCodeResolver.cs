using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Resolvers
{
    public sealed class LocationCodeResolver
    {
        private readonly Dictionary<string, LocationCode> _locationsByName;

        public LocationCodeResolver(IEnumerable<ReferenceLocationDefinition> locations)
        {
            var locationsByName = locations
                .GroupBy(
                    location => location.DisplayName,
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

            var ambiguousNames = locationsByName
                .Where(location => location.Count() > 1)
                .Select(location => location.Key)
                .ToList();

            if (ambiguousNames.Count != 0)
            {
                throw new ReferenceDataCompilationException(
                    "Location names must be unique for relationship resolution. " +
                    $"Ambiguous names: {string.Join(", ", ambiguousNames)}");
            }

            _locationsByName = locationsByName.ToDictionary(
                location => location.Key,
                location => location.Single().Code,
                StringComparer.OrdinalIgnoreCase);
        }

        public LocationCode Resolve(string locationName)
        {
            if (string.IsNullOrWhiteSpace(locationName))
            {
                throw new ArgumentException("Location name cannot be null or whitespace.", nameof(locationName));
            }

            var normalizedLocationName = StringHelpers.NormalizeWhitespace(locationName, nameof(locationName));

            if (_locationsByName.TryGetValue(normalizedLocationName, out var code))
            {
                return code;
            }

            throw new ReferenceDataCompilationException($"Location with name '{locationName}' not found.");
        }
    }
}
