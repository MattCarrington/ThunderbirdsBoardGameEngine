namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    internal interface IFileReader
    {
        ValueTask<Stream> OpenReadAsync(string path, CancellationToken cancellationToken);
    }
}
