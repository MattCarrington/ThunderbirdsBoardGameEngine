using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Application.Mappers;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal sealed class ReferenceCharacterContributionLookup : ICharacterContributionLookup
    {
        private readonly ICharacterDefinitionCatalog _characterDefinitionCatalog;

        public ReferenceCharacterContributionLookup(ICharacterDefinitionCatalog characterDefinitionCatalog)
        {
            _characterDefinitionCatalog = characterDefinitionCatalog ?? throw new ArgumentNullException(nameof(characterDefinitionCatalog));
        }

        public CharacterContribution GetCharacterContribution(CharacterCode characterCode)
        {
            try
            {
                var definition = _characterDefinitionCatalog.GetByCode(characterCode);

                var rescueBonusContribution = definition.RescueBonus is null
                        ? null
                        : new CharacterRescueBonusContribution(definition.RescueBonus.RescueType, definition.RescueBonus.Value);

                return new CharacterContribution(characterCode, rescueBonusContribution);
            }
            catch (KeyNotFoundException)
            {
                throw new ReferenceDataNotFoundException("Character", characterCode.ToString());
            }
        }
    }
}
