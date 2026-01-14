using ThunderbirdsBoardGameEngine.Catalog.Format.Hashing;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities
{
    internal class GeneratedContentValidator : IGeneratedContentValidator
    {
        public void Validate<TItem>(Payload<GeneratedCatalogManifest> payload)
        {
            var manifest = payload.Manifest;
            var dataElement = payload.RawData;

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
        }
    }
}
