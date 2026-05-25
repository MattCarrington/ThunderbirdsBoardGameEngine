using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    /// <summary>
    /// Provides read-only access to map edge definitions from the reference data snapshot.
    /// </summary>
    public interface IMapEdgeDefinitionCatalog
    {
        /// <summary>
        /// Gets all map edge definitions.
        /// </summary>
        /// <returns>An immutable array of all map edge definitions.</returns>
        ImmutableArray<ReferenceMapEdgeDefinition> GetAll();
    }
}
