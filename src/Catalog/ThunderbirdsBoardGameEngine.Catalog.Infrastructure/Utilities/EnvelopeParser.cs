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

        public async Task<Payload> ReadEnvelopeAsync(Stream stream, CancellationToken cancellationToken)
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

            var manifest = JsonSerializer.Deserialize<CatalogManifest>(manifestElement, ManifestReadOptions)
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

            if (manifest.ItemCount <= 0)
            {
                throw new InvalidDataException("Manifest must specify a positive item count");
            }

            if (dataElement.GetArrayLength() != manifest.ItemCount)
            {
                throw new InvalidDataException($"Item count mismatch: manifest specifies {manifest.ItemCount}, but 'data' contains {dataElement.GetArrayLength()} items.");
            }

            if (string.IsNullOrWhiteSpace(manifest.Checksum.Algorithm))
            {
                throw new InvalidDataException("Checksum algorithm is missing or empty.");
            }

            if (!string.Equals(CatalogChecksum.Algorithm, manifest.Checksum.Algorithm, StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException($"Unsupported checksum algorithm: {manifest.Checksum.Algorithm}");
            }

            if (string.IsNullOrWhiteSpace(manifest.Checksum.Value))
            {
                throw new InvalidDataException("Checksum value is missing or empty.");
            }

            var computedChecksum = CatalogChecksum.ComputeForDataElement(dataElement);

            if (!string.Equals(computedChecksum, manifest.Checksum.Value, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("Data checksum does not match manifest.");
            }

            return new Payload(manifest, dataElement.Clone());
        }
    }
}
