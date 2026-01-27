using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    /// <summary>
    /// Provides methods for retrieving character definitions.
    /// </summary>
    public interface ICharacterDefinitionService
    {
        /// <summary>
        /// Retrieves all available character definitions.
        /// </summary>
        /// <returns>An immutable array containing all character definitions. The array will be empty if no character definitions
        /// are available.</returns>
        ImmutableArray<CharacterDefinition> GetAll();
    }
}