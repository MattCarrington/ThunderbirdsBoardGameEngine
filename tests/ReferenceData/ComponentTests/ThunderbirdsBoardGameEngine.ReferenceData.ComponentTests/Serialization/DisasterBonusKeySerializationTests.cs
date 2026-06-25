using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Serialization
{
    public class DisasterBonusKeySerializationTests
    {
        [Fact]
        public void IsSerializedAsItsStringValue()
        {
            // Arrange
            var disasterBonusKey = new DisasterBonusKey("bonus-123");

            // Act
            var result = JsonTestSerializer.Serialize(disasterBonusKey);

            // Assert
            Assert.Equal("\"bonus-123\"", result);
        }

        [Fact]
        public void IsDeserializedFromItsStringValue()
        {
            // Arrange
            var json = "\"bonus-456\"";

            // Act
            var result = JsonTestSerializer.Deserialize<DisasterBonusKey>(json);

            // Assert
            Assert.Equal("bonus-456", result.Value);
        }

        [Fact]
        public void RoundTripsThroughJson()
        {
            // Arrange
            var original = new DisasterBonusKey("bonus-789");

            // Act
            var json = JsonTestSerializer.Serialize(original);
            var result = JsonTestSerializer.Deserialize<DisasterBonusKey>(json);

            // Assert
            Assert.Equal(original, result);
        }

        [Fact]
        public void IsSerializedAsStringWhenNestedInObject()
        {
            // Arrange
            var testObject = new TestObject
            {
                Key = new DisasterBonusKey("bonus-abc"),
                Name = "Special Bonus"
            };

            // Act
            var result = JsonTestSerializer.Serialize(testObject);

            // Assert
            Assert.Contains("\"key\": \"bonus-abc\"", result);
            Assert.Contains("\"name\": \"Special Bonus\"", result);
        }

        [Fact]
        public void IsDeserializedFromStringWhenNestedInObject()
        {
            // Arrange
            var json = "{\"key\":\"bonus-def\",\"name\":\"Extra Bonus\"}";

            // Act
            var result = JsonTestSerializer.Deserialize<TestObject>(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("bonus-def", result.Key.Value);
            Assert.Equal("Extra Bonus", result.Name);
        }

        [Fact]
        public void InvalidJsonTokenThrowsJsonException()
        {
            // Arrange
            var json = "123";

            // Act & Assert
            Assert.ThrowsAny<JsonException>(() => JsonTestSerializer.Deserialize<DisasterBonusKey>(json));
        }

        [Fact]
        public void InvalidStringValueThrowsArgumentException()
        {
            // Arrange
            var json = "\"\"";

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => JsonTestSerializer.Deserialize<DisasterBonusKey>(json));
        }

        private class TestObject
        {
            public DisasterBonusKey Key { get; init; }

            public string Name { get; init; } = null!;
        }
    }
}