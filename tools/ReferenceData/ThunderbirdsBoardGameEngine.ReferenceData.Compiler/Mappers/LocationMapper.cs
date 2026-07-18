using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Mappers
{
    public sealed class LocationMapper
    {
        public IEnumerable<ReferenceLocationDefinition> Map(IEnumerable<LocationInput> inputs)
        {
            return inputs.Select(MapLocation);
        }

        private ReferenceLocationDefinition MapLocation(LocationInput input)
        {
            return new ReferenceLocationDefinition(
                code: new LocationCode(StringHelpers.Slugify(input.Name)),
                displayName: StringHelpers.NormalizeWhitespace(input.Name, nameof(input.Name)),
                domain: Enum.Parse<MovementDomain>(input.Domain, true));
        }
    }
}
