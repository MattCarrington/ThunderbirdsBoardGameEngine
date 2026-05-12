using System.Text;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Deserializers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.UnitTests.Deserializers
{
    public class JsonSnapshotDeserializerTests
    {
        private readonly JsonSnapshotDeserializer _deserializer = new();

        [Fact]
        public void Deserialize_WhenSnapshotJsonIsValid_ReturnsSnapshot()
        {
            // Arrange
            var json = """
            {
              "schemaVersion": 1,
              "characters": []
            }
            """;

            using var stream = ToStream(json);

            // Act
            var result = _deserializer.Deserialize(stream);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.SchemaVersion);
        }

        [Fact]
        public void Deserialize_WhenJsonIsInvalid_ThrowsJsonException()
        {
            // Arrange
            using var stream = ToStream("{ not valid json }");

            // Act & Assert
            Assert.Throws<JsonException>(() => _deserializer.Deserialize(stream));
        }

        [Fact]
        public void Deserialize_WhenJsonIsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            using var stream = ToStream("null");

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _deserializer.Deserialize(stream));

            Assert.Equal("Failed to deserialize snapshot.", ex.Message);
        }

        private static MemoryStream ToStream(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value));
        }
    }
}
