using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Characters;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Mappers
{
    /// <summary>
    /// Provides methods for converting between published character codes and domain character representations.
    /// </summary>
    /// <remarks>This class offers utility methods to map between the external character code values and their
    /// corresponding domain model representations. It is intended for use when translating data between external
    /// sources and the application's internal character model. All methods are static and the class cannot be
    /// instantiated.</remarks>
    [Obsolete("Catalog API is deprecated. Use Reference Data instead")]
    public static class CharacterCodeMapper
    {
        /// <summary>
        /// Converts a specified character code to its corresponding domain character value.
        /// </summary>
        /// <param name="code">The character code to convert to a domain character. Must be a defined value of <see cref="CharacterCode"/>.</param>
        /// <returns>The <see cref="Character"/> value that corresponds to the specified character code.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="code"/> is not a recognized value of <see cref="CharacterCode"/>.</exception>
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

        /// <summary>
        /// Converts a specified character to its corresponding published character code.
        /// </summary>
        /// <param name="character">The character to convert to a published character code.</param>
        /// <returns>The published character code that corresponds to the specified character.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified character does not have a corresponding published character code.</exception>
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