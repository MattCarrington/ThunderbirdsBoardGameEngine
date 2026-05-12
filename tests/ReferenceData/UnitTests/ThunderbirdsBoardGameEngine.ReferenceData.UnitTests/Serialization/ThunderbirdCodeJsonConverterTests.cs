using System.Text;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Serialization;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Serialization
{
    public class ThunderbirdCodeJsonConverterTests
    {
        private readonly ThunderbirdCodeJsonConverter _converter;
        private readonly JsonSerializerOptions _options;

        public ThunderbirdCodeJsonConverterTests()
        {
            _converter = new ThunderbirdCodeJsonConverter();
            _options = new JsonSerializerOptions
            {
                Converters = { _converter }
            };
        }

        [Fact]
        public void Write_WhenGivenThunderbirdCode_WritesStringValue()
        {
            // Arrange
            var thunderbirdCode = new ThunderbirdCode("thunderbird-1");
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            // Act
            _converter.Write(writer, thunderbirdCode, _options);
            writer.Flush();

            var json = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.Equal("\"thunderbird-1\"", json);
        }

        [Fact]
        public void Read_WhenGivenStringValue_ReturnsThunderbirdCode()
        {
            // Arrange
            var json = "\"thunderbird-2\"";
            var bytes = Encoding.UTF8.GetBytes(json);
            var reader = new Utf8JsonReader(bytes);
            reader.Read();

            // Act
            var result = _converter.Read(ref reader, typeof(ThunderbirdCode), _options);

            // Assert
            Assert.Equal("thunderbird-2", result.Value);
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
                _converter.Read(ref reader, typeof(ThunderbirdCode), _options);
            });
        }
    }
}
