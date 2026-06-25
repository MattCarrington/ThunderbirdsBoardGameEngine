using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers
{
    public sealed class LocationCodeResolver
    {
        private readonly Dictionary<string, LocationCode> _locationsByName;

        public LocationCodeResolver(IEnumerable<ReferenceLocationDefinition> locations)
        {
            _locationsByName = locations.ToDictionary(
                location => location.DisplayName,
                location => location.Code,
                StringComparer.OrdinalIgnoreCase
            );
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
