using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Hashing;
using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders
{
    public class EnvelopeStreamBuilder
    {
        private string _dataJson = "[]";
        private readonly string _catalog = "DisasterCards";
        private string _schemaVersion = "1.0";
        private readonly string _contentVersion = "1.0.0";
        private readonly DateTime _generatedAtUtc = DateTime.UtcNow;
        private string _checksumAlgorithm = CatalogChecksum.Algorithm;
        private int? _itemCountOverride;
        private string? _checksumOverride;

        public EnvelopeStreamBuilder WithData(string data)
        {
            _dataJson = data;
            return this;
        }

        public EnvelopeStreamBuilder WithSchemaVersion(string schemaVersion)
        {
            _schemaVersion = schemaVersion;
            return this;
        }

        public EnvelopeStreamBuilder WithItemCountOverride(int itemCount)
        {
            _itemCountOverride = itemCount;
            return this;
        }

        public EnvelopeStreamBuilder WithChecksumAlgorithm(string algorithm)
        {
            _checksumAlgorithm = algorithm;
            return this;
        }

        public EnvelopeStreamBuilder WithChecksumOverride(string checksum)
        {
            _checksumOverride = checksum;
            return this;
        }

        public Stream CreateStream()
        {
            var (_, checksum) = CreateValidData(_dataJson);

            var manifest = new
            {
                catalog = _catalog,
                schemaVersion = _schemaVersion,
                contentVersion = _contentVersion,
                generatedAtUtc = _generatedAtUtc,
                itemCount = _itemCountOverride ?? 1,
                checksum = new
                {
                    algorithm = _checksumAlgorithm,
                    value = _checksumOverride ?? checksum
                },

            };

            var manifestJson = JsonSerializer.Serialize(manifest, CatalogJson.Catalog);

            using var manifestDoc = JsonDocument.Parse(manifestJson);
            using var dataDoc = JsonDocument.Parse(_dataJson);

            using var buffer = new MemoryStream();
            using (var writer = new Utf8JsonWriter(buffer))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("meta");
                manifestDoc.RootElement.WriteTo(writer);
                writer.WritePropertyName("data");
                dataDoc.RootElement.WriteTo(writer);
                writer.WriteEndObject();
            }

            buffer.Position = 0;

            return new MemoryStream(buffer.ToArray()); // return independent stream
        }

        private static (JsonElement Data, string Checksum) CreateValidData(string dataJson)
        {
            using var document = JsonDocument.Parse(dataJson);

            var dataElement = document.RootElement.Clone();

            var checksum = CatalogChecksum.ComputeForDataElement(dataElement);

            return (dataElement, checksum);
        }
    }
}
