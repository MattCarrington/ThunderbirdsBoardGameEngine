using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Serialization
{
    public class CardCodeSerializationTests
    {
        [Fact]
        public void IsSerializedAsItsStringValue()
        {
            {
                // Arrange
                var cardCode = new CardCode("attack-of-the-aligators");

                // Act
                var result = JsonTestSerializer.Serialize(cardCode);

                // Assert
                Assert.Equal("\"attack-of-the-aligators\"", result);
            }
        }

        [Fact]
        public void IsDeserializedFromItsStringValue()
        {
            // Arrange
            var json = "\"the-imposters\"";

            // Act
            var result = JsonTestSerializer.Deserialize<CardCode>(json);

            // Assert
            Assert.Equal("the-imposters", result.Value);
        }

        [Fact]
        public void RoundTripsThroughJson()
        {
            // Arrange
            var original = new CardCode("the-perils-of-penelope");

            // Act
            var json = JsonTestSerializer.Serialize(original);
            var result = JsonTestSerializer.Deserialize<CardCode>(json);

            // Assert
            Assert.Equal(original, result);
        }

        [Fact]
        public void IsSerializedAsStringWhenNestedInObject()
        {
            // Arrange
            var testObject = new TestObject
            {
                Code = new CardCode("the-uninvited"),
                Name = "The Uninvited"
            };

            // Act
            var result = JsonTestSerializer.Serialize(testObject);

            // Assert
            Assert.Contains("\"code\": \"the-uninvited\"", result);
            Assert.Contains("\"name\": \"The Uninvited\"", result);
        }

        [Fact]
        public void IsDeserializedFromStringWhenNestedInObject()
        {
            // Arrange
            var json = "{\"code\":\"flight-path-to-fear\",\"name\":\"Flight Path to Fear\"}";

            // Act
            var result = JsonTestSerializer.Deserialize<TestObject>(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("flight-path-to-fear", result.Code.Value);
            Assert.Equal("Flight Path to Fear", result.Name);
        }

        [Fact]
        public void InvalidJsonTokenThrowsJsonException()
        {
            // Arrange
            var json = "123";

            // Act & Assert
            Assert.ThrowsAny<JsonException>(() => JsonTestSerializer.Deserialize<CardCode>(json));
        }

        [Fact]
        public void InvalidStringValueThrowsArgumentException()
        {
            // Arrange
            var json = "\"\"";

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => JsonTestSerializer.Deserialize<CardCode>(json));
        }

        private class TestObject
        {
            public CardCode Code { get; init; }

            public string Name { get; init; } = null!;
        }
    }
}
