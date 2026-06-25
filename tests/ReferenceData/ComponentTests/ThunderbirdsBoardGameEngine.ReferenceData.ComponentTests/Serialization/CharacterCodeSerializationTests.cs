using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Serialization
{
    public class CharacterCodeSerializationTests
    {
        [Fact]
        public void IsSerializedAsItsStringValue()
        {
            // Arrange
            var characterCode = new CharacterCode("scott");

            // Act
            var result = JsonTestSerializer.Serialize(characterCode);

            // Assert
            Assert.Equal("\"scott\"", result);
        }

        [Fact]
        public void IsDeserializedFromItsStringValue()
        {
            // Arrange
            var json = "\"virgil\"";

            // Act
            var result = JsonTestSerializer.Deserialize<CharacterCode>(json);

            // Assert
            Assert.Equal("virgil", result.Value);
        }

        [Fact]
        public void RoundTripsThroughJson()
        {
            // Arrange
            var original = new CharacterCode("alan");

            // Act
            var json = JsonTestSerializer.Serialize(original);
            var result = JsonTestSerializer.Deserialize<CharacterCode>(json);

            // Assert
            Assert.Equal(original, result);
        }

        [Fact]
        public void IsSerializedAsStringWhenNestedInObject()
        {
            // Arrange
            var testObject = new TestObject
            {
                Code = new CharacterCode("gordon"),
                Name = "Gordon"
            };

            // Act
            var result = JsonTestSerializer.Serialize(testObject);

            // Assert
            Assert.Contains("\"code\": \"gordon\"", result);
            Assert.Contains("\"name\": \"Gordon\"", result);
        }

        [Fact]
        public void IsDeserializedFromStringWhenNestedInObject()
        {
            // Arrange
            var json = "{\"code\":\"john\",\"name\":\"John\"}";

            // Act
            var result = JsonTestSerializer.Deserialize<TestObject>(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("john", result.Code.Value);
            Assert.Equal("John", result.Name);
        }

        [Fact]
        public void InvalidJsonTokenThrowsJsonException()
        {
            // Arrange
            var json = "123";

            // Act & Assert
            Assert.ThrowsAny<JsonException>(() => JsonTestSerializer.Deserialize<CharacterCode>(json));
        }

        [Fact]
        public void InvalidStringValueThrowsArgumentException()
        {
            // Arrange
            var json = "\"\"";

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => JsonTestSerializer.Deserialize<CharacterCode>(json));
        }

        private class TestObject
        {
            public CharacterCode Code { get; init; }

            public string Name { get; init; } = null!;
        }
    }
}