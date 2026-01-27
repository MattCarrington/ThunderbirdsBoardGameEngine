using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Client.Infrastructure;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1
{
    /// <summary>
    /// Defines a client for retrieving character data from the API.
    /// </summary>
    /// /// <remarks>
    /// This client is versioned via the HTTP pipeline and sends an <c>X-Api-Version</c> header.
    /// Register with <see cref="Extensions.ServiceCollectionExtensions.AddCatalogClients"/>. 
    /// </remarks>
    public interface ICharactersClient
    {
        /// <summary>
        /// Asynchronously retrieves all characters.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ApiResult{T}"/>
        /// with a read-only list of <see cref="CharacterDto"/> objects representing all characters. The list is empty
        /// if no characters are found.</returns>
        Task<ApiResult<IReadOnlyList<CharacterDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
