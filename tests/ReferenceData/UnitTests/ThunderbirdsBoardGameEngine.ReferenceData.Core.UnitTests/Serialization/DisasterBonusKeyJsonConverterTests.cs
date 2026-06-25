using System.Text;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Serialization;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Serialization
{
    public class DisasterBonusKeyJsonConverterTests
    {
        private readonly DisasterBonusKeyJsonConverter _converter;
        private readonly JsonSerializerOptions _options;

        public DisasterBonusKeyJsonConverterTests()
        {
            _converter = new DisasterBonusKeyJsonConverter();
            _options = new JsonSerializerOptions
            {
                Converters = { _converter }
            };
        }

        [Fact]
        public void Write_WhenGivenDisasterBonusKey_WritesStringValue()
        {
            // Arrange
            var bonusKey = new DisasterBonusKey("scott");
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            // Act
            _converter.Write(writer, bonusKey, _options);
            writer.Flush();

            var json = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.Equal("\"scott\"", json);
        }

        [Fact]
        public void Read_WhenGivenStringValue_ReturnsDisasterBonusKey()
        {
            // Arrange
            var json = "\"thunderbird-4\"";
            var bytes = Encoding.UTF8.GetBytes(json);
            var reader = new Utf8JsonReader(bytes);
            reader.Read();

            // Act
            var result = _converter.Read(ref reader, typeof(DisasterBonusKey), _options);

            // Assert
            Assert.Equal("thunderbird-4", result.Value);
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
                _converter.Read(ref reader, typeof(DisasterBonusKey), _options);
            });
        }
    }
}
