using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    /// <summary>
    /// Represents a service that provides asynchronous access to character definitions.
    /// </summary>
    public interface ICharacterDefinitionReader
    {
        /// <summary>
        /// Asynchronously retrieves all available character definitions.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of all
        /// character definitions. The list will be empty if no character definitions are available.</returns>
        Task<IReadOnlyList<CharacterDefinition>> GetAllAsync(CancellationToken cancellationToken);
    }
}
