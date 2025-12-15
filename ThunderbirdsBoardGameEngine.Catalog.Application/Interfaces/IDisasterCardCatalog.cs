using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    /// <summary>
    /// Represents a read-only view of the disaster card catalog.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface exposes disaster card data for inspection and querying
    /// without allowing modification.
    /// </para>
    /// </remarks>
    public interface IDisasterCardCatalog
    {
        /// <summary>
        /// Gets the version of the catalog.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets all disaster cards in the catalog.
        /// </summary>
        ImmutableArray<DisasterCard> Cards { get; }

        DisasterCard GetById(int id);
    }
}
