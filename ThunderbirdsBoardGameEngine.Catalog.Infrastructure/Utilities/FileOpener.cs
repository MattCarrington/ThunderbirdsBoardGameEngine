using System.IO.Abstractions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities
{
    internal sealed class FileOpener : IFileOpener
    {
        private readonly IFileSystem _fileSystem;

        public FileOpener(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or whitespace.", nameof(path));
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<Stream>(cancellationToken);
            }

            var stream = _fileSystem.FileStream.New(
                path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, options: FileOptions.Asynchronous | FileOptions.SequentialScan);

            return Task.FromResult<Stream>(stream);
        }
    }
}
