using ThunderbirdsBoardGameEngine.Api.Presentation;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Api.Mappers.Catalog.V1
{
    public static class CharacterDefinitionMappingExtensions
    {
        public static CharacterDto ToDto(this CharacterDefinition characterDefinition)
        {
            return new CharacterDto
            {
                Key = characterDefinition.Key.ToString(),
                DisplayName = EnumDisplayHelper.GetDisplayName(characterDefinition.Key)
            };
        }

        public static IReadOnlyList<CharacterDto> ToDto(this IEnumerable<CharacterDefinition> characterDefinitions)
        {
            return characterDefinitions.Select(cd => cd.ToDto()).ToList();
        }
    }
}
