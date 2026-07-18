using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Mappers
{
    public sealed class FabCardMapper
    {
        public IEnumerable<ReferenceFabCardDefinition> Map(IEnumerable<FabCardInput> inputs)
        {
            return inputs.Select(MapFabCard);
        }

        private ReferenceFabCardDefinition MapFabCard(FabCardInput input)
        {
            return new ReferenceFabCardDefinition(
                code: new CardCode(StringHelpers.Slugify(input.Name)),
                displayName: StringHelpers.NormalizeWhitespace(input.Name, nameof(input.Name)));
        }
    }
}
