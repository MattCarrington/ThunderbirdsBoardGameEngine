using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    /// <summary>
    /// Provides read access to disaster card domain entities.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implementations are responsible for retrieving disaster cards from a
    /// data source and returning them as domain entities.
    /// </para>
    /// <para>
    /// This interface represents a data access boundary and may be implemented
    /// by infrastructure components.
    /// </para>
    /// </remarks>
    public interface IDisasterCardReader
    {
        /// <summary>
        /// Retrieves all disaster cards.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token used to cancel the operation.
        /// </param>
        /// <returns>
        /// A read-only collection of disaster cards.
        /// </returns>
        /// <exception cref="CatalogDataAccessException">
        /// Thrown when the underlying data source cannot be accessed.
        /// </exception>
        Task<IReadOnlyList<DisasterCard>> GetAllAsync(CancellationToken cancellationToken);
    }
}