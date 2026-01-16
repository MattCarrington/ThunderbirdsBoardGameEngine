using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PayloadReaders
{
    internal class JsonCatalogPayloadReader<TManifest> : ICatalogPayloadReader<TManifest> where TManifest : ICatalogManifest
    {
        private readonly ICatalogStreamSource _catalogStreamSource;
        private readonly IJsonStreamValidator _jsonStreamValidator;
        private readonly IEnvelopeParser _envelopeParser;

        public JsonCatalogPayloadReader(ICatalogStreamSource catalogStreamSource, IJsonStreamValidator jsonStreamValidator, IEnvelopeParser envelopeParser)
        {
            _catalogStreamSource = catalogStreamSource ?? throw new ArgumentNullException(nameof(catalogStreamSource));
            _jsonStreamValidator = jsonStreamValidator ?? throw new ArgumentNullException(nameof(jsonStreamValidator));
            _envelopeParser = envelopeParser ?? throw new ArgumentNullException(nameof(envelopeParser));
        }

        public async Task<Payload<TManifest>> ReadAsync(string filePath, CancellationToken cancellationToken)
        {
            await using var stream = await _catalogStreamSource.OpenReadAsync(filePath, cancellationToken);
            await using var validated = await _jsonStreamValidator.ValidateStreamAsync(stream, filePath, cancellationToken);

            return await _envelopeParser.ReadEnvelopeAsync<TManifest>(validated, cancellationToken);
        }
    }
}
