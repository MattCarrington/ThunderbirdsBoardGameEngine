using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.TestUtils;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.UnitTests.Converters
{
    public class BonusConditionCatalogDtoConversionTests
    {
        private readonly JsonSerializerOptions _options = JsonDefaults.DisasterCards;

        [Fact]
        public void Deserialize_WhenCharacterBonusJson_ReturnsExpectedCharacterBonusCatalogDto()
        {
            // Arrange
            var input = """
            {
                "type": "characterBonus",
                "bonusValue": 2,
                "character": "virgil"
            }
            """;

            // Act
            var result = DeserializeBonusFromJson(input);

            // Assert
            var characterBonus = Assert.IsType<CharacterBonusCatalogDto>(result);
            Assert.Equal(2, characterBonus.BonusValue);
            Assert.Equal("Virgil", characterBonus.Character, ignoreCase: true);
            Assert.Null(characterBonus.Location);
        }

        [Fact]
        public void Deserialize_WhenCharacterBonusWithLocationJson_ReturnsExpectedCharacterBonusCatalogDto()
        {
            // Arrange
            var input = """
            {
                "type": "characterBonus",
                "bonusValue": 2,
                "character": "virgil",
                "location": "indianOcean"
            }
            """;

            // Act
            var result = DeserializeBonusFromJson(input);

            // Assert
            var characterBonus = Assert.IsType<CharacterBonusCatalogDto>(result);
            Assert.Equal(2, characterBonus.BonusValue);
            Assert.Equal("Virgil", characterBonus.Character, ignoreCase: true);
            Assert.Equal("IndianOcean", characterBonus.Location, ignoreCase: true);
        }

        [Fact]
        public void Deserialize_WhenThunderbirdBonusJson_ReturnsExpectedThunderbirdBonus()
        {
            // Arrange
            var input = """
            {
                "type": "thunderbirdBonus",
                "bonusValue": 2,
                "thunderbird": "thunderbird4"
            }
            """;

            // Act
            var result = DeserializeBonusFromJson(input);

            // Assert
            var thunderbirdBonus = Assert.IsType<ThunderbirdBonusCatalogDto>(result);
            Assert.Equal(2, thunderbirdBonus.BonusValue);
            Assert.Equal("Thunderbird4", thunderbirdBonus.Thunderbird, ignoreCase: true);
            Assert.Null(thunderbirdBonus.Location);
        }

        [Fact]
        public void Deserialize_WhenThunderbirdBonusWithLocationJson_ReturnsExpectedThunderbirdBonus()
        {
            // Arrange
            var input = """
            {
                "type": "thunderbirdBonus",
                "bonusValue": 2,
                "thunderbird": "thunderbird4",
                "location": "northPacific"
            }
            """;

            // Act
            var result = DeserializeBonusFromJson(input);

            // Assert
            var thunderbirdBonus = Assert.IsType<ThunderbirdBonusCatalogDto>(result);
            Assert.Equal(2, thunderbirdBonus.BonusValue);
            Assert.Equal("Thunderbird4", thunderbirdBonus.Thunderbird, ignoreCase: true);
            Assert.Equal("NorthPacific", thunderbirdBonus.Location, ignoreCase: true);
        }

        [Fact]        
        public void Deserialize_WhenPodVehicleBonusJson_ReturnsExpectedPodVehicleBonusCatalogDto()
        {
            // Arrange
            var input = """
            {
                "type": "podVehicleBonus",
                "bonusValue": 2,
                "podVehicle": "elevatorCars"
            }
            """;            

            // Act
            var result = DeserializeBonusFromJson(input);

            // Assert
            var podVehicleBonus = Assert.IsType<PodVehicleBonusCatalogDto>(result);
            Assert.Equal(2, podVehicleBonus.BonusValue);
            Assert.Equal("ElevatorCars", podVehicleBonus.PodVehicle, ignoreCase: true);
            Assert.Null(podVehicleBonus.Location);
        }

        [Fact]
        public void Deserialize_WhenPodVehicleBonusWithLocationJson_ReturnsExpectedPodVehicleBonusCatalogDto()
        {
            // Arrange
            var input = """
            {
                "type": "podVehicleBonus",
                "bonusValue": 2,
                "podVehicle": "elevatorCars",
                "location": "southAtlantic"
            }
            """;

            // Act
            var result = DeserializeBonusFromJson(input);

            // Assert
            var podVehicleBonus = Assert.IsType<PodVehicleBonusCatalogDto>(result);
            Assert.Equal(2, podVehicleBonus.BonusValue);
            Assert.Equal("ElevatorCars", podVehicleBonus.PodVehicle, ignoreCase: true);
            Assert.Equal("SouthAtlantic", podVehicleBonus.Location, ignoreCase: true);
        }                

        [Fact]
        public void Deserialize_WhenUnknownType_ThrowsJsonException()
        {
            // Arrange
            var input = """
            {
                "type": "unknownBonus",
                "bonusValue": 2,
                "unknown": "unknown"
            }
            """;

            // Act & Assert
            Assert.Throws<JsonException>(() => DeserializeBonusFromJson(input));
        }

        [Fact]
        public void Deserialize_WhenMissingType_ThrowsJsonException()
        {
            // Arrange
            var input = """
            {
                "bonusValue": 2,
                "unknown": "unknown"
            }
            """;

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => DeserializeBonusFromJson(input));
        }

        [Fact]
        public void Serialize_WhenCharacterBonusCatalogDto_WritesExpectedJson()
        {
            // Arrange
            var bonus = new CharacterBonusCatalogDto()
            {
                Character = "scott",
                BonusValue = 3,
                Location = "indianOcean"
            };

            // Act
            var result = SerializeBonusToJson(bonus);

            // Assert
            Assert.Equal("characterBonus", result.GetProperty("type").GetString());
            Assert.Equal(3, result.GetProperty("bonusValue").GetInt32());
            Assert.Equal("scott", result.GetProperty("character").GetString());
            Assert.Equal("indianOcean", result.GetProperty("location").GetString());
        }

        [Fact]
        public void Serialize_WhenCharacterBonusCatalogDtoWithoutLocation_WritesExpectedJson()
        {
            // Arrange
            var bonus = new CharacterBonusCatalogDto()
            {
                Character = "alan",
                BonusValue = 5,
                Location = null
            };
            
            // Act
            var result = SerializeBonusToJson(bonus); // assuming your helper method uses .Clone()

            // Assert
            Assert.Equal("characterBonus", result.GetProperty("type").GetString());
            Assert.Equal(5, result.GetProperty("bonusValue").GetInt32());
            Assert.Equal("alan", result.GetProperty("character").GetString());
            Assert.False(result.TryGetProperty("location", out _), "Location should not be present in JSON");
        }

        [Fact]
        public void Serialize_WhenThunderbirdBonusCatalogDto_WritesExpectedJson()
        {
            // Arrange
            var bonus = new ThunderbirdBonusCatalogDto()
            {
                Thunderbird = "thunderbird2",
                BonusValue = 4,
                Location = "northPacific"
            };

            // Act
            var result = SerializeBonusToJson(bonus);

            // Assert
            Assert.Equal("thunderbirdBonus", result.GetProperty("type").GetString());
            Assert.Equal(4, result.GetProperty("bonusValue").GetInt32());
            Assert.Equal("thunderbird2", result.GetProperty("thunderbird").GetString());
            Assert.Equal("northPacific", result.GetProperty("location").GetString());
        }

        [Fact]
        public void Serialize_WhenThunderbirdBonusCatalogDtoWithoutLocation_WritesExpectedJson()
        {
            // Arrange
            var bonus = new ThunderbirdBonusCatalogDto()
            {
                Thunderbird = "thunderbird3",
                BonusValue = 3,
                Location = null
            };

            // Act
            var result = SerializeBonusToJson(bonus);

            // Assert
            Assert.Equal("thunderbirdBonus", result.GetProperty("type").GetString());
            Assert.Equal(3, result.GetProperty("bonusValue").GetInt32());
            Assert.Equal("thunderbird3", result.GetProperty("thunderbird").GetString());
            Assert.False(result.TryGetProperty("location", out _), "Location should not be present in JSON");
        }

        [Fact]
        public void Serialize_WhenPodVehicleBonusCatalogDto_WritesExpectedJson()
        {
            // Arrange
            var bonus = new PodVehicleBonusCatalogDto()
            {
                PodVehicle = "elevatorCars",
                BonusValue = 1,
                Location = "southAtlantic"
            };

            // Act
            var result = SerializeBonusToJson(bonus);

            // Assert
            Assert.Equal("podVehicleBonus", result.GetProperty("type").GetString());
            Assert.Equal(1, result.GetProperty("bonusValue").GetInt32());
            Assert.Equal("elevatorCars", result.GetProperty("podVehicle").GetString());
            Assert.Equal("southAtlantic", result.GetProperty("location").GetString());
        }

        [Fact]
        public void Serialize_WhenPodVehicleBonusCatalogDtoWithoutLocation_WritesExpectedJson()
        {
            // Arrange
            var bonus = new PodVehicleBonusCatalogDto()
            {
                PodVehicle = "mole",
                BonusValue = 3,
                Location = null
            };

            // Act
            var result = SerializeBonusToJson(bonus);

            // Assert
            Assert.Equal("podVehicleBonus", result.GetProperty("type").GetString());
            Assert.Equal(3, result.GetProperty("bonusValue").GetInt32());
            Assert.Equal("mole", result.GetProperty("podVehicle").GetString());
            Assert.False(result.TryGetProperty("location", out _), "Location should not be present in JSON");
        }
        
        [Fact]
        public void Serialize_WhenInvalidBonus_ThrowsJsonException()
        {
            // Arrange
            var bonus = new FakeBonus()
            {
                FakeProperty = "Invalid",
                BonusValue = 1
            };

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => SerializeBonusToJson(bonus));
        }

        [Fact]
        public void RoundTrip_WhenCharacterBonusCatalogDto_SerializesAndDeserializesCorrectly()
        {
            // Arrange
            var characterBonus = new CharacterBonusCatalogDto()
            {
                Character = "Virgil",
                BonusValue = 2,
                Location = "IndianOcean"
            };

            // Act
            var json = SerializeBonusToJson(characterBonus);
            var result = DeserializeBonusFromJson(json.GetRawText());

            // Assert
            var bonus = Assert.IsType<CharacterBonusCatalogDto>(result);
            Assert.Equal(characterBonus.BonusValue, bonus.BonusValue);
            Assert.Equal(characterBonus.Character, bonus.Character);
            Assert.Equal(characterBonus.Location, bonus.Location);
        }

        [Fact]
        public void RoundTrip_WhenThunderbirdBonusCatalogDto_SerializesAndDeserializesCorrectly()
        {
            // Arrange
            var thunderbirdBonus = new ThunderbirdBonusCatalogDto()
            {
                Thunderbird = "Thunderbird1",
                BonusValue = 3,
                Location = "NorthAtlantic"
            };

            // Act
            var json = SerializeBonusToJson(thunderbirdBonus);
            var result = DeserializeBonusFromJson(json.GetRawText());

            // Assert
            var bonus = Assert.IsType<ThunderbirdBonusCatalogDto>(result);
            Assert.Equal(thunderbirdBonus.BonusValue, bonus.BonusValue);
            Assert.Equal(thunderbirdBonus.Thunderbird, bonus.Thunderbird);
            Assert.Equal(thunderbirdBonus.Location, bonus.Location);
        }

        [Fact]
        public void RoundTrip_WhenPodVehicleBonusCatalogDto_SerializesAndDeserializesCorrectly()
        {
            // Arrange
            var podVehicleBonus = new PodVehicleBonusCatalogDto()
            {
                PodVehicle = "Mole",
                BonusValue = 4,
                Location = "SouthPacific"
            };

            // Act
            var json = SerializeBonusToJson(podVehicleBonus);
            var result = DeserializeBonusFromJson(json.GetRawText());

            // Assert
            var bonus = Assert.IsType<PodVehicleBonusCatalogDto>(result);
            Assert.Equal(podVehicleBonus.BonusValue, bonus.BonusValue);
            Assert.Equal(podVehicleBonus.PodVehicle, bonus.PodVehicle);
            Assert.Equal(podVehicleBonus.Location, bonus.Location);
        }

        private BonusConditionCatalogDto DeserializeBonusFromJson(string json)
        {
            return JsonSerializer.Deserialize<BonusConditionCatalogDto>(json, _options)!;
        }

        private JsonElement SerializeBonusToJson(BonusConditionCatalogDto bonus)
        {
            var json = JsonSerializer.Serialize(bonus, _options);

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }

        private record FakeBonus : BonusConditionCatalogDto
        {
            [JsonConstructor]
            public FakeBonus()
            {                
            }

            public string? FakeProperty { get; set; }
        }
    }
}
