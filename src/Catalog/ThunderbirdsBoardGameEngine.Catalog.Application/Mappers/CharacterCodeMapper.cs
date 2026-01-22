using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Characters;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Mappers
{
    public static class CharacterCodeMapper
    {
        public static Character ToDomain(CharacterCode code)
        {
            return code switch
            {
                var c when c == CharacterCode.Scott => Character.Scott,
                var c when c == CharacterCode.Virgil => Character.Virgil,
                var c when c == CharacterCode.Alan => Character.Alan,
                var c when c == CharacterCode.Gordon => Character.Gordon,
                var c when c == CharacterCode.John => Character.John,
                var c when c == CharacterCode.LadyPenelope => Character.LadyPenelope,
                _ => throw new ArgumentOutOfRangeException(nameof(code))
            };
        }

        public static CharacterCode ToPublished(Character character)
        {
            return character switch
            {
                Character.Scott => CharacterCode.Scott,
                Character.Virgil => CharacterCode.Virgil,
                Character.Alan => CharacterCode.Alan,
                Character.Gordon => CharacterCode.Gordon,
                Character.John => CharacterCode.John,
                Character.LadyPenelope => CharacterCode.LadyPenelope,
                _ => throw new ArgumentOutOfRangeException(nameof(character))
            };
        }
    }
}