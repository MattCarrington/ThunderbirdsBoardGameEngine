using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

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
                // Technically, changing the exception type thrown here would be a breaking change to the API,
                // as the handler for this exception type returns a Bad Request, while the handler for the original exception type returns a Not Found.
                // However, as the contract is still evolving, it should be safe to do so.
                throw new InvalidRescueCalculationRequestException("Character", characterCode.ToString());
            }
        }
    }
}
