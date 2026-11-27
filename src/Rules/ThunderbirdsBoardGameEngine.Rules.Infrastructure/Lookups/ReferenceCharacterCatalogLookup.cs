using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Contributions;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal sealed class ReferenceCharacterCatalogLookup : ICharacterCatalogLookup
    {
        private readonly ICharacterDefinitionCatalog _characterDefinitionCatalog;

        public ReferenceCharacterCatalogLookup(ICharacterDefinitionCatalog characterDefinitionCatalog)
        {
            _characterDefinitionCatalog = characterDefinitionCatalog ?? throw new ArgumentNullException(nameof(characterDefinitionCatalog));
        }

        public CharacterContribution GetCharacterRescueContribution(CharacterCode characterCode)
        {
            if (!_characterDefinitionCatalog.TryGetByCode(characterCode, out var characterDefinition))
            {
                throw new InvalidRescueCalculationRequestException("Character", characterCode.ToString());
            }

            var rescueBonusContribution = characterDefinition.RescueBonus is null
                ? null
                : new CharacterRescueBonusContribution(characterDefinition.RescueBonus.RescueType, characterDefinition.RescueBonus.Value);

            return new CharacterContribution(characterCode, rescueBonusContribution);
        }
    }
}
