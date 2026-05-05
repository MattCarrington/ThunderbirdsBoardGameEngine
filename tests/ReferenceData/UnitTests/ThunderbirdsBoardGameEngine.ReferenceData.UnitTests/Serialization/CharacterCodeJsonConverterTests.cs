using System.Text;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Serialization;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Serialization
{
    public class CharacterCodeJsonConverterTests
    {
        private readonly CharacterCodeJsonConverter _converter;
        private readonly JsonSerializerOptions _options;

        public CharacterCodeJsonConverterTests()
        {
            _converter = new CharacterCodeJsonConverter();
            _options = new JsonSerializerOptions
            {
                Converters = { _converter }
            };
        }

        [Fact]
        public void Write_WhenGivenCharacterCode_WritesStringValue()
        {
            // Arrange
            var characterCode = new CharacterCode("scott");
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            // Act
            _converter.Write(writer, characterCode, _options);
            writer.Flush();

            var json = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.Equal("\"scott\"", json);
        }

        [Fact]
        public void Read_WhenGivenStringValue_ReturnsCharacterCode()
        {
            // Arrange
            var json = "\"virgil\"";
            var bytes = Encoding.UTF8.GetBytes(json);
            var reader = new Utf8JsonReader(bytes);
            reader.Read();

            // Act
            var result = _converter.Read(ref reader, typeof(CharacterCode), _options);

            // Assert
            Assert.Equal("virgil", result.Value);
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
