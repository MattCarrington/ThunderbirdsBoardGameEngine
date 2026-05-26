using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    /// <summary>
    /// Provides read-only access to Thunderbird definitions from the reference data snapshot.
    /// </summary>
    public interface IThunderbirdDefinitionCatalog
    {

        /// <summary>
        /// Gets a Thunderbird definition by its unique code.
        /// </summary>
        /// <param name="code">The Thunderbird code.</param>
        /// <returns>The Thunderbird definition.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no Thunderbird with the specified code exists.
        /// </exception>
        ReferenceThunderbirdDefinition GetByCode(ThunderbirdCode code);
    }
}