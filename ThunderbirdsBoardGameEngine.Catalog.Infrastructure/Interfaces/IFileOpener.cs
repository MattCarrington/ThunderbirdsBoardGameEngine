namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    internal interface IFileOpener
    {
        Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken);
    }
}
