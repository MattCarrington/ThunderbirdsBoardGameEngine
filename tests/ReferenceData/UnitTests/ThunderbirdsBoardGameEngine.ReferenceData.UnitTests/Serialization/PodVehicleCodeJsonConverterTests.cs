using System.Text;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Serialization;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Serialization
{
    public class PodVehicleCodeJsonConverterTests
    {
        private readonly PodVehicleCodeJsonConverter _converter;
        private readonly JsonSerializerOptions _options;

        public PodVehicleCodeJsonConverterTests()
        {
            _converter = new PodVehicleCodeJsonConverter();
            _options = new JsonSerializerOptions
            {
                Converters = { _converter }
            };
        }

        [Fact]
        public void Write_WhenGivenPodVehicleCode_WritesStringValue()
        {
            // Arrange
            var podVehicleCode = new PodVehicleCode("mole");
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            // Act
            _converter.Write(writer, podVehicleCode, _options);
            writer.Flush();

            var json = Encoding.UTF8.GetString(stream.ToArray());

            // Assert
            Assert.Equal("\"mole\"", json);
        }

        [Fact]
        public void Read_WhenGivenStringValue_ReturnsPodVehicleCode()
        {
            // Arrange
            var json = "\"firefly\"";
            var bytes = Encoding.UTF8.GetBytes(json);
            var reader = new Utf8JsonReader(bytes);
            reader.Read();

            // Act
            var result = _converter.Read(ref reader, typeof(PodVehicleCode), _options);

            // Assert
            Assert.Equal("firefly", result.Value);
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
                _converter.Read(ref reader, typeof(PodVehicleCode), _options);
            });
        }
    }
}
