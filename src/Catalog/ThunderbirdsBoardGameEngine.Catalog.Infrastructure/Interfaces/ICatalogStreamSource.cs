namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    internal interface ICatalogStreamSource
    {
        Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken);
    }
}
