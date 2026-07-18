using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Mappers
{
    public sealed class CharacterMapper
    {
        public IEnumerable<ReferenceCharacterDefinition> Map(IEnumerable<CharacterInput> inputs)
        {
            return inputs.Select(MapCharacter);
        }

        private ReferenceCharacterDefinition MapCharacter(CharacterInput input)
        {
            ReferenceCharacterRescueBonus? rescueBonus = null;

            if (!string.IsNullOrWhiteSpace(input.RescueType) && input.BonusValue.HasValue)
            {
                var rescueType = Enum.Parse<RescueType>(input.RescueType, ignoreCase: true);
                rescueBonus = new ReferenceCharacterRescueBonus(rescueType, input.BonusValue.Value);
            }

            return new ReferenceCharacterDefinition(
                code: new CharacterCode(StringHelpers.Slugify(input.Name)),
                displayName: StringHelpers.NormalizeWhitespace(input.Name, nameof(input.Name)),
                rescueBonus: rescueBonus);
        }
    }
}
