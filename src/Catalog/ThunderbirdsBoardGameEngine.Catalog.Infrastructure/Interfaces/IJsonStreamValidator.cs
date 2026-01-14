namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    internal interface IJsonStreamValidator
    {
        Task<Stream> ValidateStreamAsync(Stream stream, string filePath, CancellationToken cancellationToken);
    }
}