using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Hashing;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities
{
    internal sealed class EnvelopeParser : IEnvelopeParser
    {
        private static readonly JsonSerializerOptions ManifestReadOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<Payload<TManifest>> ReadEnvelopeAsync<TManifest>(Stream stream, CancellationToken cancellationToken) where TManifest : ICatalogManifest
        {
            using var document = await JsonDocument.ParseAsync(stream, new JsonDocumentOptions
            {
                AllowTrailingCommas = false
            },
            cancellationToken);

            var root = document.RootElement;

            if (!root.TryGetProperty("meta", out var manifestElement))
            {
                throw new InvalidDataException("Envelope is missing 'meta' property.");
            }

            if (!root.TryGetProperty("data", out var dataElement))
            {
                throw new InvalidDataException("Envelope is missing 'data' property.");
            }

            var manifest = JsonSerializer.Deserialize<TManifest>(manifestElement, ManifestReadOptions)
                ?? throw new InvalidDataException("Failed to deserialize 'manifest'.");

            if (string.IsNullOrWhiteSpace(manifest.SchemaVersion))
            {
                throw new InvalidDataException("Schema version is missing or empty.");
            }

            if (!manifest.SchemaVersion.StartsWith("1.", StringComparison.Ordinal))
            {
                throw new NotSupportedException($"Unsupported schema version: {manifest.SchemaVersion}");
            }

            if (dataElement.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidDataException("'data' property is not an array.");
            }

            return new Payload<TManifest>
            {
                Manifest = manifest,
                RawData = dataElement.Clone()
            };
        }
    }
}
