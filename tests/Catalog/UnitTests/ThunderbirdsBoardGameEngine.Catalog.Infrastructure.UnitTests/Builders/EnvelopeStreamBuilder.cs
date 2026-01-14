using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Hashing;
using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders
{
    public class EnvelopeStreamBuilder
    {
        private string _dataJson = "[]";
        private string _schemaVersion = "1.0";

        private readonly string _catalog = "DisasterCards";
        private readonly string _contentVersion = "1.0.0";
        private readonly DateTime _generatedAtUtc = DateTime.UtcNow;
        private readonly string _checksumAlgorithm = CatalogChecksum.Algorithm;

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

        public Stream CreateStream()
        {
            var manifest = new
            {
                catalog = _catalog,
                schemaVersion = _schemaVersion,
                contentVersion = _contentVersion,
                generatedAtUtc = _generatedAtUtc,
                itemCount = 1,
                checksum = new
                {
                    algorithm = _checksumAlgorithm,
                    value = "somechecksumvalue"
                },
                toolInfo = new
                {
                    name = "Catalog.Infrastructure.UnitTests",
                    version = "1.0.0-test"
                }
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
    }
}
