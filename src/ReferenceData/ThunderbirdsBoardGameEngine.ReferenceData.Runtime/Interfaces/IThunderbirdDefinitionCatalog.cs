using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    /// <summary>
    /// Provides read-only access to Thunderbird definitions from the reference data snapshot.
    /// </summary>
    public interface IThunderbirdDefinitionCatalog
    {

        /// <summary>
        /// Tries to get a Thunderbird definition by its unique code.
        /// </summary>
        /// <param name="code">The Thunderbird code.</param>
        /// <param name="definition">The Thunderbird definition if found; otherwise, null.</param>
        /// <returns>True if the Thunderbird definition was found; otherwise, false.</returns>

        bool TryGetByCode(
            ThunderbirdCode code,
            [NotNullWhen(true)]
            out ReferenceThunderbirdDefinition? definition);

        /// <summary>
        /// Gets all Thunderbird definitions in the catalog.
        /// </summary>
        /// <returns></returns>
        ImmutableArray<ReferenceThunderbirdDefinition> GetAll();
    }
}