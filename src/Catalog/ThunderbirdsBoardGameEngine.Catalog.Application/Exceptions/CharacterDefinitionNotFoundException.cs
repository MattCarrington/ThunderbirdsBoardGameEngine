using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions
{
    public sealed class CharacterDefinitionNotFoundException : NotFoundException
    {
        /// <summary>
        /// Gets the name of character associated with the missing definition.
        /// </summary>
        public Character Character { get; }

        /// <summary>
        /// Initializes a new instance of the CharacterDefinitionNotFoundException class for the specified character.
        /// </summary>
        /// <param name="character">The character whose definition could not be found.</param>
        public CharacterDefinitionNotFoundException(Character character)
            : base($"Character definition for {character} was not found.", "CharacterDefinition", character.ToString())
        {
            Character = character;
        }
    }
}
