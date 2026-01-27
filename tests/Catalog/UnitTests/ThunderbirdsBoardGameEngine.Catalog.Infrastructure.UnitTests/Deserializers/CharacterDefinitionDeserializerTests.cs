using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Deserializers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Stubs;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Deserializers
{
    public class CharacterDefinitionDeserializerTests
    {
        [Fact]
        public void Deserialize_WhenValidJson_ReturnsCharacterDtos()
        {
            // Arrange
            var json = """
            [
                { 
                    "key": "scott", 
                    "rescueBonuses": [
                         { 
                            "rescueType": "air",
                            "bonusValue": 2 
                        } 
                    ]
                    
                },
                { 
                    "key": "ladypenelope",
                    "rescueBonuses": []
                }
            ]
            """;

            var element = JsonParser.ParseElement(json);

            var deserializer = CreateDeserializer();

            // Act
            var result = deserializer.Deserialize(element);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Key == "scott" && c.RescueBonuses.Count == 1 && c.RescueBonuses[0].RescueType == "air" && c.RescueBonuses[0].BonusValue == 2);
            Assert.Contains(result, c => c.Key == "ladypenelope" && c.RescueBonuses.Count == 0);
        }

        [Fact]
        public void Deserialize_WhenEmptyArray_ReturnsEmptyList()
        {
            // Arrange
            var json = "[]";

            var element = JsonParser.ParseElement(json);

            var deserializer = CreateDeserializer();

            // Act
            var result = deserializer.Deserialize(element);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Deserialize_WhenNullJson_ReturnsEmptyList()
        {
            // Arrange
            var element = JsonParser.ParseElement($"null");

            var deserializer = CreateDeserializer();

            // Act
            var result = deserializer.Deserialize(element);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        private static CharacterDefinitionDeserializer CreateDeserializer()
        {
            var options = JsonOptionsStub.CatalogOptionsMonitor();

            return new CharacterDefinitionDeserializer(options);
        }
    }
}
