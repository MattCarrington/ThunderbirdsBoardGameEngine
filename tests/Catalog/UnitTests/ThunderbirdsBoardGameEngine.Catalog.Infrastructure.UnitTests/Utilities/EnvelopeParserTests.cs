using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Utilities
{
    public class EnvelopeParserTests
    {
        [Fact]
        public async Task ReadEnvelopeAsync_WhenValid_ReturnsPayload()
        {
            // Arrange
            var data = ValidData();

            using var stream = new EnvelopeStreamBuilder().WithData(data).CreateStream();

            var parser = CreateParser();

            // Act
            var result = await parser.ReadEnvelopeAsync<GeneratedCatalogManifest>(stream, CancellationToken.None);

            // Assert
            Assert.NotNull(result.Manifest);
            Assert.Equal(1, result.RawData.GetArrayLength());
        }

        [Fact]
        public async Task ReadEnvelopeAsync_WhenNoManifest_ThrowsInvalidDataException()
        {
            // Arrange
            using var stream = StreamFrom("""{ "data": [] }""");

            var parser = CreateParser();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(() => parser.ReadEnvelopeAsync<GeneratedCatalogManifest>(stream, CancellationToken.None));
        }

        [Fact]
        public async Task ReadEnvelopeAsync_WhenNoData_ThrowsInvalidDataException()
        {
            // Arrange
            using var stream = StreamFrom("""
            {
              "meta": {
                "catalog": "DisasterCards",
                "schemaVersion": "1.0",
                "contentVersion": "1970-01-01",
                "generatedAtUtc": "1970-01-01T00:00:00Z",
                "itemCount": 0,
                "checksum": { "algorithm": "sha256", "value": "00" }
              }
            }
            """);

            var parser = new EnvelopeParser();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(() => parser.ReadEnvelopeAsync<GeneratedCatalogManifest>(stream, CancellationToken.None));
        }

        [Fact]
        public async Task ReadEnvelopeAsync_WhenManifestInvalid_ThrowsInvalidDataException()
        {
            // Arrange
            using var stream = StreamFrom("""
            { "meta": null, "data": [] }
            """);

            var parser = CreateParser();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(() => parser.ReadEnvelopeAsync<GeneratedCatalogManifest>(stream, CancellationToken.None));
        }

        [Fact]
        public async Task ReadEnvelopeAsync_WhenCatalogVersionInvalid_ThrowsInvalidDataException()
        {
            // Arrange
            var data = ValidData();

            using var stream = new EnvelopeStreamBuilder()
                .WithData(data)
                .WithSchemaVersion("2.0")
                .CreateStream();

            var parser = CreateParser();

            // Act & Assert
            await Assert.ThrowsAsync<NotSupportedException>(() => parser.ReadEnvelopeAsync<GeneratedCatalogManifest>(stream, CancellationToken.None));
        }

        [Fact]
        public async Task ReadEnvelopeAsync_WhenDataNotArray_ThrowsInvalidDataException()
        {
            // Arrange
            var data = """
            { "id": 1, "name": "Storm", "code": "storm", "location": "Africa", "rescueType": "Air", "difficultyNumber": 3 }
            """;

            using var stream = new EnvelopeStreamBuilder().WithData(data).CreateStream();

            var parser = CreateParser();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(() => parser.ReadEnvelopeAsync<GeneratedCatalogManifest>(stream, CancellationToken.None));
        }

        [Fact]
        public async Task ReadEnvelopeAsync_WhenCancellationRequested_ThrowsOperationCanceledException()
        {
            // Arrange
            var data = ValidData();

            using var stream = new EnvelopeStreamBuilder().WithData(data).CreateStream();

            var parser = CreateParser();

            using var token = new CancellationTokenSource();
            await token.CancelAsync();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => parser.ReadEnvelopeAsync<GeneratedCatalogManifest>(stream, token.Token));
        }

        [Theory]
        [ClassData(typeof(WhiteSpaceStringData))]
        public async Task ReadEnvelopeAsync_WhenSchemaValueIsNullOrWhitespace_ThrowsInvalidDataException(string? invalidSchemaVersion)
        {
            // Arrange
            var data = ValidData();

            using var stream = new EnvelopeStreamBuilder()
                .WithData(data)
                .WithSchemaVersion(invalidSchemaVersion)
                .CreateStream();

            var parser = CreateParser();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(() => parser.ReadEnvelopeAsync<GeneratedCatalogManifest>(stream, CancellationToken.None));
        }

        private static string ValidData()
        {
            return """
            [
                { "id": 1, "name": "Storm", "code": "storm", "location": "Africa", "rescueType": "Air", "difficultyNumber": 3 }
            ]
            """;
        }

        private static MemoryStream StreamFrom(string json)
        {
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
        }

        private static EnvelopeParser CreateParser()
        {
            return new EnvelopeParser();
        }
    }
}
