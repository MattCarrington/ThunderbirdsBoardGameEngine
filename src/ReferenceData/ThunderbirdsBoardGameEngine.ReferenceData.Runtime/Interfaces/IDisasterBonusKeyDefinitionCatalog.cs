using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Models;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    /// <summary>
    /// Provides read-only access to disaster bonus key definitions from the reference data snapshot.
    /// </summary>
    public interface IDisasterBonusKeyDefinitionCatalog
    {
        /// <summary>
        /// Gets a disaster bonus key definition by its unique key.
        /// </summary>
        /// <param name="key">The disaster bonus key.</param>
        /// <returns>The disaster bonus key definition.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no disaster bonus key with the specified key exists.
        /// </exception>
        DisasterBonusKeyDefinition GetByCode(DisasterBonusKey key);
    }
}