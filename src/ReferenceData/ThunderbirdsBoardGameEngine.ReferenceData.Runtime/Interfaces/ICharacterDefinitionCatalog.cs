using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    /// <summary>
    /// Provides read-only access to character definitions from the reference data snapshot.
    /// </summary>
    public interface ICharacterDefinitionCatalog
    {
        /// <summary>
        /// Gets all character definitions.
        /// </summary>
        /// <returns>An immutable array of all character definitions.</returns>
        ImmutableArray<ReferenceCharacterDefinition> GetAll();

        /// <summary>
        /// Gets a character definition by its unique code.
        /// </summary>
        /// <param name="code">The character code.</param>
        /// <returns>The character definition.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no character with the specified code exists.
        /// </exception>
        ReferenceCharacterDefinition GetByCode(CharacterCode code);
    }
}