using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;

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
    [Obsolete("Catalog API is deprecated. Use Reference Data instead")]
    public interface IDisasterCardReferenceSource
    {
        /// <summary>
        /// Gets the version of the catalog.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets all disaster cards in the catalog.
        /// </summary>
        ImmutableArray<DisasterCard> Cards { get; }

        DisasterCard GetByCode(CardCode disasterCardCode);
    }
}
