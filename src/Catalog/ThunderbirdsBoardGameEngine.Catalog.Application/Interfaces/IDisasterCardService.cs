using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    /// <summary>
    /// Provides application-level operations for working with disaster cards.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This service represents the application use cases for disaster cards
    /// and enforces application and domain invariants.
    /// </para>
    /// </remarks>
    [Obsolete("Catalog API is deprecated. Use Reference Data instead")]
    public interface IDisasterCardService
    {
        /// <summary>
        /// Returns all disaster cards.
        /// </summary>
        /// <returns>
        /// An immutable collection of disaster cards.
        /// </returns>
        /// <exception cref="ApplicationValidationException">
        /// Thrown when application-level validation fails.
        /// </exception>
        /// <exception cref="CatalogDataAccessException">
        /// Thrown when catalog data cannot be retrieved.
        /// </exception>
        ImmutableArray<DisasterCard> GetAll();
    }
}