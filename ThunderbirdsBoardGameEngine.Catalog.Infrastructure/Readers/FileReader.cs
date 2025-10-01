using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers
{
    internal sealed class FileReader : IFileReader
    {
        public ValueTask<Stream> OpenReadAsync(string path, CancellationToken cancellationToken)
        {
            return new(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
                                          FileOptions.Asynchronous | FileOptions.SequentialScan));
        }
    }
}
