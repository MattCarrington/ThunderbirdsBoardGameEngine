using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Deserializers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Stubs;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Deserializers
{
    public class DisasterCardDeserializerTests
    {
        [Fact]
        public void Deserialize_WhenValidJson_ReturnsDisasterCardDtos()
        {
            // Arrange
            var json = """
            [
                { 
                    "id": 1, 
                    "name": "Storm",
                    "code": "storm",
                    "location": "Africa", 
                    "rescueType": "Air", 
                    "difficultyNumber": 3, 
                    "bonusConditions": [ 
                        { 
                            "type": "characterBonus", 
                            "character": "Scott", 
                            "bonusValue": 3 
                        } 
                    ],
                    "rewardOptions": [
                        {
                            "type": "token",
                            "token": "technology"
                        }
                    ]
                },
                { 
                    "id": 2, 
                    "name": "Quake",
                    "code": "quake",
                    "location": "SouthPacific", 
                    "rescueType": "Land", 
                    "difficultyNumber": 7,
                    "bonusConditions": [ 
                        { 
                            "type": "thunderbirdBonus", 
                            "thunderbird": "Thunderbird3", 
                            "bonusValue": 3,
                            "location": "theSun"
                        },
                        {                        
                            "type": "podVehicleBonus", 
                            "podVehicle": "mole", 
                            "bonusValue": 2 
                        }
                    ],
                    "rewardOptions": [
                        {
                            "type": "playerChoice"
                        },
                        {
                            "type": "token",
                            "token": "logistics"
                        }
                    ]
                }
            ]
            """;

            var element = JsonParser.ParseElement(json);

            var deserializer = CreateDeserializer();

            // Act
            var result = deserializer.Deserialize(element);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, d => d.Id == 1 && d.Name == "Storm");
            Assert.Contains(result, d => d.Id == 2 && d.Name == "Quake");
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

        private static DisasterCardDeserializer CreateDeserializer()
        {
            var options = JsonOptionsStub.CatalogOptionsMonitor();

            return new DisasterCardDeserializer(options);
        }
    }
}
