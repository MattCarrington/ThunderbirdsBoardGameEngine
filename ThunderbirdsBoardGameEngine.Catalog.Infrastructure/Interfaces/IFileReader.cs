namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    internal interface IFileReader
    {
        Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken);
    }
}
