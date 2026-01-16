using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Helpers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Mappers
{
    internal sealed class CharacterMapper : ICharacterMapper
    {
        public CharacterDefinition Map(CharacterCatalogDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var character = EnumParser.ParseEnum<Character>(dto.Key);

            // Enforce cardinality at the mapping boundary
            if (dto.RescueBonuses is { Count: > 1 })
            {
                throw new ArgumentException(
                    $"Character '{dto.Key}' has more than one rescue bonus.");
            }

            CharacterRescueBonus? rescueBonus = null;

            if (dto.RescueBonuses?.Count == 1)
            {
                rescueBonus = MapRescueBonus(dto.RescueBonuses[0]);
            }

            return new CharacterDefinition(character, rescueBonus);
        }

        private static CharacterRescueBonus MapRescueBonus(CharacterRescueBonusCatalogDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var rescueType = EnumParser.ParseEnum<RescueType>(dto.RescueType);

            return new CharacterRescueBonus(
                rescueType,
                dto.BonusValue);
        }
    }
}
