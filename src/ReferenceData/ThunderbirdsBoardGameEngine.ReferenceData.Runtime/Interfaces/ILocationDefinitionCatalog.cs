using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    /// <summary>
    /// Provides read-only access to location definitions from the reference data snapshot.
    /// </summary>
    public interface ILocationDefinitionCatalog
    {
        /// <summary>
        /// Gets all location definitions.
        /// </summary>
        /// <returns>An immutable array of all location definitions.</returns>
        ImmutableArray<ReferenceLocationDefinition> GetAll();

        /// <summary>
        /// Checks if a location definition with the specified code exists.
        /// </summary>
        /// <param name="code">The location code.</param>
        /// <returns>True if the location definition exists; otherwise, false.</returns>
        bool Exists(LocationCode code);

        /// <summary>
        /// Gets a location definition by its unique code.
        /// </summary>
        /// <param name="code">The location code.</param>
        /// <returns>The location definition.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no location with the specified code exists.
        /// </exception>
        ReferenceLocationDefinition GetByCode(LocationCode code);
    }
}