using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    internal interface IEnvelopeParser
    {
        Task<Payload> ReadEnvelopeAsync(Stream stream, CancellationToken cancellationToken);
    }
}