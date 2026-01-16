using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.StreamSources
{
    internal class FileSystemCatalogStreamSource : ICatalogStreamSource
    {
        private readonly IFileOpener _fileOpener;

        public FileSystemCatalogStreamSource(IFileOpener fileOpener)
        {
            _fileOpener = fileOpener ?? throw new ArgumentNullException(nameof(fileOpener));
        }

        public async Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken)
        {
            return await _fileOpener.OpenReadAsync(path, cancellationToken);
        }
    }
}
