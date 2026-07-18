using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Mappers
{
    public sealed class EventCardMapper
    {
        public IEnumerable<ReferenceEventCardDefinition> Map(IEnumerable<EventCardInput> inputs)
        {
            return inputs.Select(MapEventCard);
        }

        private ReferenceEventCardDefinition MapEventCard(EventCardInput input)
        {
            return new ReferenceEventCardDefinition(
                code: new CardCode(StringHelpers.Slugify(input.Name)),
                displayName: StringHelpers.NormalizeWhitespace(input.Name, nameof(input.Name)));
        }
    }
}
