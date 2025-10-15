using System.Text;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Fakes
{
    public sealed class FakeFileOpener : IFileOpener
    {
        private readonly Dictionary<string, Func<CancellationToken, Task<Stream>>> _map = new(StringComparer.OrdinalIgnoreCase);

        public FakeFileOpener Add(string path, string content)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path is required.", nameof(path));
            }

            var bytes = Encoding.UTF8.GetBytes(content);

            _map[path] = cancellationToken =>
            {
                if (cancellationToken.IsCancellationRequested) 
                { 
                    return Task.FromCanceled<Stream>(cancellationToken); 
                }

                return Task.FromResult<Stream>(new MemoryStream(bytes, writable: false));
            };

            return this;
        }

        public FakeFileOpener AddCanceled(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path is required.", nameof(path));
            }

            _map[path] = cancellationToken => Task.FromCanceled<Stream>(
                cancellationToken.IsCancellationRequested ? cancellationToken : new CancellationToken(true));

            return this;
        }

        public FakeFileOpener AddException(string path, Exception ex)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path is required.", nameof(path));
            }

            _map[path] = _ => Task.FromException<Stream>(ex);

            return this;
        }

        public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return Task.FromException<Stream>(new ArgumentException("Path is required.", nameof(path)));
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<Stream>(cancellationToken);
            }

            if (_map.TryGetValue(path, out var factory))
            {
                return factory(cancellationToken);
            }

            return Task.FromException<Stream>(new FileNotFoundException(path));
        }
    }
}
