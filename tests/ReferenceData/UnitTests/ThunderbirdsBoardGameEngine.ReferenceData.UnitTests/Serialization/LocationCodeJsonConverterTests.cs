using System.Text;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Serialization;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Serialization
{
    public class LocationCodeJsonConverterTests
    {
        private readonly LocationCodeJsonConverter _converter;
        private readonly JsonSerializerOptions _options;

        public LocationCodeJsonConverterTests()
        {
            _converter = new LocationCodeJsonConverter();
            _options = new JsonSerializerOptions
            {
                Converters = { _converter }
            };
        }

        [Fact]
        public void Write_WhenGivenLocationCode_WritesStringValue()
        {
            // Arrange
            var locationCode = new LocationCode("europe");
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            // Act
            _converter.Write(writer, locationCode, _options);
            writer.Flush();

            var json = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.Equal("\"europe\"", json);
        }

        [Fact]
        public void Read_WhenGivenStringValue_ReturnsLocationCode()
        {
            // Arrange
            var json = "\"north-america\"";
            var bytes = Encoding.UTF8.GetBytes(json);
            var reader = new Utf8JsonReader(bytes);
            reader.Read();

            // Act
            var result = _converter.Read(ref reader, typeof(LocationCode), _options);

            // Assert
            Assert.Equal("north-america", result.Value);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("true")]
        [InlineData("{}")]
        [InlineData("[]")]
        public void Read_WhenTokenIsNotString_ThrowsJsonException(string json)
        {
            // Arrange
            var bytes = Encoding.UTF8.GetBytes(json);

            // Act & Assert
            Assert.Throws<JsonException>(() =>
            {
                var reader = new Utf8JsonReader(bytes);
                reader.Read();
                _converter.Read(ref reader, typeof(LocationCode), _options);
            });
        }
    }
}
