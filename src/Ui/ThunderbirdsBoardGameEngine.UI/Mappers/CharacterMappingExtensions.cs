using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Mappers
{
    public static class CharacterMappingExtensions
    {
        public static CharacterViewModel ToViewModel(this ReferenceCharacterDefinition character)
        {
            return new CharacterViewModel(
                Key: character.Code.ToString(),
                DisplayName: character.DisplayName
            );
        }
    }
}
