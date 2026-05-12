using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Serialization
{
    public class LocationCodeSerializationTests
    {
        [Fact]
        public void IsSerializedAsItsStringValue()
        {
            // Arrange
            var locationCode = new LocationCode("london");

            // Act
            var result = JsonTestSerializer.Serialize(locationCode);

            // Assert
            Assert.Equal("\"london\"", result);
        }

        [Fact]
        public void IsDeserializedFromItsStringValue()
        {
            // Arrange
            var json = "\"paris\"";

            // Act
            var result = JsonTestSerializer.Deserialize<LocationCode>(json);

            // Assert
            Assert.Equal("paris", result.Value);
        }

        [Fact]
        public void RoundTripsThroughJson()
        {
            // Arrange
            var original = new LocationCode("new-york");

            // Act
            var json = JsonTestSerializer.Serialize(original);
            var result = JsonTestSerializer.Deserialize<LocationCode>(json);

            // Assert
            Assert.Equal(original, result);
        }

        [Fact]
        public void IsSerializedAsStringWhenNestedInObject()
        {
            // Arrange
            var testObject = new TestObject
            {
                Code = new LocationCode("tokyo"),
                Name = "Tokyo"
            };

            // Act
            var result = JsonTestSerializer.Serialize(testObject);

            // Assert
            Assert.Contains("\"code\": \"tokyo\"", result);
            Assert.Contains("\"name\": \"Tokyo\"", result);
        }

        [Fact]
        public void IsDeserializedFromStringWhenNestedInObject()
        {
            // Arrange
            var json = "{\"code\":\"sydney\",\"name\":\"Sydney\"}";

            // Act
            var result = JsonTestSerializer.Deserialize<TestObject>(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("sydney", result.Code.Value);
            Assert.Equal("Sydney", result.Name);
        }

        [Fact]
        public void InvalidJsonTokenThrowsJsonException()
        {
            // Arrange
            var json = "123";

            // Act & Assert
            Assert.ThrowsAny<JsonException>(() => JsonTestSerializer.Deserialize<LocationCode>(json));
        }

        [Fact]
        public void InvalidStringValueThrowsArgumentException()
        {
            // Arrange
            var json = "\"\"";

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => JsonTestSerializer.Deserialize<LocationCode>(json));
        }

        private class TestObject
        {
            public LocationCode Code { get; init; }

            public string Name { get; init; } = null!;
        }
    }
}