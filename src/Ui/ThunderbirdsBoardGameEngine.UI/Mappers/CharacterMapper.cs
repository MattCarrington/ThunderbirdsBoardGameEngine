using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Mappers
{
    public class CharacterMapper
    {
        public CharacterViewModel ToViewModel(ReferenceCharacterDefinition character)
        {
            return new CharacterViewModel(
                Key: character.Code.ToString(),
                DisplayName: character.DisplayName
            );
        }
    }
}
