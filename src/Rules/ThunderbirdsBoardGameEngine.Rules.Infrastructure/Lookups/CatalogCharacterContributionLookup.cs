using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Application.Mappers;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Characters;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal sealed class CatalogCharacterContributionLookup : ICharacterContributionLookup
    {
        private readonly ICharacterDefinitionReferenceSource _characterDefinitionReferenceSource;

        public CatalogCharacterContributionLookup(ICharacterDefinitionReferenceSource characterDefinitionReferenceSource)
        {
            _characterDefinitionReferenceSource = characterDefinitionReferenceSource ?? throw new ArgumentNullException(nameof(characterDefinitionReferenceSource));
        }

        public CharacterContribution GetCharacterContribution(CharacterCode characterCode)
        {
            var character = CharacterCodeMapper.ToDomain(characterCode);

            var definition = _characterDefinitionReferenceSource.GetCharacterDefinition(character);

            var rescueBonusContribution = definition.RescueBonus is null
                    ? null
                    : new CharacterRescueBonusContribution(definition.RescueBonus.RescueType, definition.RescueBonus.BonusValue);

            return new CharacterContribution(characterCode, rescueBonusContribution);
        }
    }
}
