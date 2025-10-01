using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Fakes
{
    public class FakeFileReader : IFileReader
    {
        private readonly Dictionary<string, Func<CancellationToken, ValueTask<Stream>>> _map = new();

        public FakeFileReader Add(string path, string content)
        {
            _map[path] = _ => new ValueTask<Stream>(
                new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)));
            return this;
        }

        public FakeFileReader AddCanceled(string path)
        {
            _map[path] = ValueTask.FromCanceled<Stream>;
            return this;
        }

        public ValueTask<Stream> OpenReadAsync(string path, CancellationToken cancellationToken)
        {
            return _map.TryGetValue(path, out var factory)
                ? factory(cancellationToken)
                : ValueTask.FromException<Stream>(new FileNotFoundException(path));
        }
    }
}
