using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Serialization
{
    public class PodVehicleCodeSerializationTests
    {
        [Fact]
        public void IsSerializedAsItsStringValue()
        {
            // Arrange
            var podVehicleCode = new PodVehicleCode("mole");

            // Act
            var result = JsonTestSerializer.Serialize(podVehicleCode);

            // Assert
            Assert.Equal("\"mole\"", result);
        }

        [Fact]
        public void IsDeserializedFromItsStringValue()
        {
            // Arrange
            var json = "\"firefly\"";

            // Act
            var result = JsonTestSerializer.Deserialize<PodVehicleCode>(json);

            // Assert
            Assert.Equal("firefly", result.Value);
        }

        [Fact]
        public void RoundTripsThroughJson()
        {
            // Arrange
            var original = new PodVehicleCode("excavator");

            // Act
            var json = JsonTestSerializer.Serialize(original);
            var result = JsonTestSerializer.Deserialize<PodVehicleCode>(json);

            // Assert
            Assert.Equal(original, result);
        }

        [Fact]
        public void IsSerializedAsStringWhenNestedInObject()
        {
            // Arrange
            var testObject = new TestObject
            {
                Code = new PodVehicleCode("thunderizer"),
                Name = "Thunderizer"
            };

            // Act
            var result = JsonTestSerializer.Serialize(testObject);

            // Assert
            Assert.Contains("\"code\": \"thunderizer\"", result);
            Assert.Contains("\"name\": \"Thunderizer\"", result);
        }

        [Fact]
        public void IsDeserializedFromStringWhenNestedInObject()
        {
            // Arrange
            var json = "{\"code\":\"domo\",\"name\":\"DOMO\"}";

            // Act
            var result = JsonTestSerializer.Deserialize<TestObject>(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("domo", result.Code.Value);
            Assert.Equal("DOMO", result.Name);
        }

        [Fact]
        public void InvalidJsonTokenThrowsJsonException()
        {
            // Arrange
            var json = "123";

            // Act & Assert
            Assert.ThrowsAny<JsonException>(() => JsonTestSerializer.Deserialize<PodVehicleCode>(json));
        }

        [Fact]
        public void InvalidStringValueThrowsArgumentException()
        {
            // Arrange
            var json = "\"\"";

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => JsonTestSerializer.Deserialize<PodVehicleCode>(json));
        }

        private class TestObject
        {
            public PodVehicleCode Code { get; init; }

            public string Name { get; init; } = null!;
        }
    }
}