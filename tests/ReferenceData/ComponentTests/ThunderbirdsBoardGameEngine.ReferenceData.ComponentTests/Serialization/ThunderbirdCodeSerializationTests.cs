using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Serialization
{
    public class ThunderbirdCodeSerializationTests
    {
        [Fact]
        public void IsSerializedAsItsStringValue()
        {
            // Arrange
            var thunderbirdCode = new ThunderbirdCode("thunderbird-1");

            // Act
            var result = JsonTestSerializer.Serialize(thunderbirdCode);

            // Assert
            Assert.Equal("\"thunderbird-1\"", result);
        }

        [Fact]
        public void IsDeserializedFromItsStringValue()
        {
            // Arrange
            var json = "\"thunderbird-2\"";

            // Act
            var result = JsonTestSerializer.Deserialize<ThunderbirdCode>(json);

            // Assert
            Assert.Equal("thunderbird-2", result.Value);
        }

        [Fact]
        public void RoundTripsThroughJson()
        {
            // Arrange
            var original = new ThunderbirdCode("thunderbird-3");

            // Act
            var json = JsonTestSerializer.Serialize(original);
            var result = JsonTestSerializer.Deserialize<ThunderbirdCode>(json);

            // Assert
            Assert.Equal(original, result);
        }

        [Fact]
        public void IsSerializedAsStringWhenNestedInObject()
        {
            // Arrange
            var testObject = new TestObject
            {
                Code = new ThunderbirdCode("thunderbird-4"),
                Name = "Thunderbird 4"
            };

            // Act
            var result = JsonTestSerializer.Serialize(testObject);

            // Assert
            Assert.Contains("\"code\": \"thunderbird-4\"", result);
            Assert.Contains("\"name\": \"Thunderbird 4\"", result);
        }

        [Fact]
        public void IsDeserializedFromStringWhenNestedInObject()
        {
            // Arrange
            var json = "{\"code\":\"thunderbird-5\",\"name\":\"Thunderbird 5\"}";

            // Act
            var result = JsonTestSerializer.Deserialize<TestObject>(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("thunderbird-5", result.Code.Value);
            Assert.Equal("Thunderbird 5", result.Name);
        }

        [Fact]
        public void InvalidJsonTokenThrowsJsonException()
        {
            // Arrange
            var json = "123";

            // Act & Assert
            Assert.ThrowsAny<JsonException>(() => JsonTestSerializer.Deserialize<ThunderbirdCode>(json));
        }

        [Fact]
        public void InvalidStringValueThrowsArgumentException()
        {
            // Arrange
            var json = "\"\"";

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => JsonTestSerializer.Deserialize<ThunderbirdCode>(json));
        }

        private class TestObject
        {
            public ThunderbirdCode Code { get; init; }

            public string Name { get; init; } = null!;
        }
    }
}