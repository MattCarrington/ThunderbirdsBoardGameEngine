using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Format.Hashing;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.TestUtils;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders
{
    public class GeneratedManifestPayloadBuilder
    {
        private int _itemCount = 2;
        private string _algorithm = CatalogChecksum.Algorithm;
        private string? _checksumOverride = null;
        private string _rawDataJson = """[ { "id": "card1", "name": "Test Card" }, { "id": "card2", "name": "card-2" } ]""";

        public GeneratedManifestPayloadBuilder WithItemCount(int count)
        {
            _itemCount = count;
            return this;
        }

        public GeneratedManifestPayloadBuilder WithChecksumAlgorithm(string algorithm)
        {
            _algorithm = algorithm;
            return this;
        }

        public GeneratedManifestPayloadBuilder WithChecksumOverride(string checksum)
        {
            _checksumOverride = checksum;
            return this;
        }

        public GeneratedManifestPayloadBuilder WithDisasterCards(IReadOnlyList<DisasterCardCatalogDto> data)
        {
            _rawDataJson = JsonSerializer.Serialize(data, JsonDefaults.DisasterCards);
            _itemCount = data.Count;
            return this;
        }

        internal Payload<GeneratedCatalogManifest> Build()
        {
            using var rawDataDoc = JsonDocument.Parse(_rawDataJson);

            var checksum = CatalogChecksum.ComputeForDataElement(rawDataDoc.RootElement);

            var manifest = new GeneratedCatalogManifest
            {
                Catalog = "DisasterCards",
                SchemaVersion = "1.0",
                ContentVersion = "1.0.0",
                GeneratedAtUtc = DateTime.UtcNow,
                ItemCount = _itemCount,
                Checksum = new Checksum
                {
                    Algorithm = _algorithm,
                    Value = _checksumOverride ?? checksum
                },
                ToolInfo = new ToolInfo
                {
                    Name = "Test",
                    Version = "1.0.0"
                },
            };

            return new Payload<GeneratedCatalogManifest>
            {
                Manifest = manifest,
                RawData = rawDataDoc.RootElement.Clone()
            };
        }
    }
}
