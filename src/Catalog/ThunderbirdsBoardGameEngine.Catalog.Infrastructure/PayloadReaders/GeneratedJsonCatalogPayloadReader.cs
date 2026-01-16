using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PayloadReaders
{
    internal class GeneratedJsonCatalogPayloadReader : ICatalogPayloadReader<GeneratedCatalogManifest>
    {
        private readonly ICatalogPayloadReader<GeneratedCatalogManifest> _inner;
        private readonly IGeneratedContentValidator _generatedContentValidator;

        public GeneratedJsonCatalogPayloadReader(ICatalogPayloadReader<GeneratedCatalogManifest> inner, IGeneratedContentValidator generatedContentValidator)
        {
             _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _generatedContentValidator = generatedContentValidator ?? throw new ArgumentNullException(nameof(generatedContentValidator));
        }

        public async Task<Payload<GeneratedCatalogManifest>> ReadAsync(string filePath, CancellationToken cancellationToken)
        {
            var payload = await _inner.ReadAsync(filePath, cancellationToken);

            _generatedContentValidator.Validate<GeneratedCatalogManifest>(payload);

            return payload;
        }
    }
}
