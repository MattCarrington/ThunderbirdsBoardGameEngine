using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    /// <summary>
    /// Provides read-only access to disaster definitions from the reference data snapshot.
    /// </summary>
    public interface IDisasterDefinitionCatalog
    {
        /// <summary>
        /// Gets all disaster definitions.
        /// </summary>
        /// <returns>An immutable array of all disaster definitions.</returns>
        ImmutableArray<ReferenceDisasterDefinition> GetAll();

        /// <summary>
        /// Gets a disaster definition by its unique code.
        /// </summary>
        /// <param name="code">The disaster card code.</param>
        /// <returns>The disaster definition.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no disaster with the specified code exists.
        /// </exception>
        ReferenceDisasterDefinition GetByCode(CardCode code);
    }
}