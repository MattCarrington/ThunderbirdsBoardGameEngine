using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    internal interface ICatalogPayloadReader<TManifest> where TManifest : ICatalogManifest
    {
        Task<Payload<TManifest>> ReadAsync(string filePath, CancellationToken cancellationToken);
    }
}
