using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Mappers
{
    public sealed class ThunderbirdMapper
    {
        public IEnumerable<ReferenceThunderbirdDefinition> Map(IEnumerable<ThunderbirdInput> inputs)
        {
            return inputs.Select(MapThunderbird);
        }
        private ReferenceThunderbirdDefinition MapThunderbird(ThunderbirdInput input)
        {
            return new ReferenceThunderbirdDefinition(
                code: new ThunderbirdCode(StringHelpers.Slugify(input.Name)),
                displayName: StringHelpers.NormalizeWhitespace(input.Name, nameof(input.Name)),
                domain: Enum.Parse<MovementDomain>(input.MovementDomain, ignoreCase: true),
                topSpeed: input.TopSpeed
            );
        }
    }
}
