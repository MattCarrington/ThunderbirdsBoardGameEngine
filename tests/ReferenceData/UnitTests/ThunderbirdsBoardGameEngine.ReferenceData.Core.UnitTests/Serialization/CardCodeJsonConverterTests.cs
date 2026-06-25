using System.Text;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Serialization;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Serialization
{
    public class CardCodeJsonConverterTests
    {
        private readonly CardCodeJsonConverter _converter;
        private readonly JsonSerializerOptions _options;

        public CardCodeJsonConverterTests()
        {
            _converter = new CardCodeJsonConverter();
            _options = new JsonSerializerOptions
            {
                Converters = { _converter }
            };
        }

        [Fact]
        public void Write_WhenGivenCardCode_WritesStringValue()
        {
            // Arrange
            var cardCode = new CardCode("trapped-in-the-sky");
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            // Act
            _converter.Write(writer, cardCode, _options);
            writer.Flush();

            var json = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.Equal("\"trapped-in-the-sky\"", json);
        }

        [Fact]
        public void Read_WhenGivenStringValue_ReturnsCardCode()
        {
            // Arrange
            var json = "\"pit-of-peril\"";
            var bytes = Encoding.UTF8.GetBytes(json);
            var reader = new Utf8JsonReader(bytes);
            reader.Read();

            // Act
            var result = _converter.Read(ref reader, typeof(CardCode), _options);

            // Assert
            Assert.Equal("pit-of-peril", result.Value);
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
                _converter.Read(ref reader, typeof(CardCode), _options);
            });
        }
    }
}
