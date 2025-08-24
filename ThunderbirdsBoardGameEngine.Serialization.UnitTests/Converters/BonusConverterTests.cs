using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Serialization.Converters;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Serialization.UnitTests.Converters
{
    public class BonusConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public BonusConverterTests()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _options.Converters.Add(new JsonStringEnumConverter());
            _options.Converters.Add(new BonusConverter());
        }

        [Fact]
        public void Deserialize_WhenCharacterBonusJson_ReturnsExpectedCharacterBonus()
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
            var result = JsonSerializer.Deserialize<BonusCondition>(input, _options);

            // Assert
            var characterBonus = Assert.IsType<CharacterBonusCondition>(result);
            Assert.Equal(2, characterBonus.BonusValue);
            Assert.Equal(Character.Virgil, characterBonus.Character);
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
            var result = JsonSerializer.Deserialize<BonusCondition>(input, _options);

            // Assert
            var thunderbirdBonus = Assert.IsType<ThunderbirdBonusCondition>(result);
            Assert.Equal(2, thunderbirdBonus.BonusValue);
            Assert.Equal(ThunderbirdMachine.Thunderbird4, thunderbirdBonus.Thunderbird);
        }

        [Fact]        
        public void Deserialize_WhenPodVehicleBonusJson_ReturnsExpectedPodVehicleBonus()
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
            var result = JsonSerializer.Deserialize<BonusCondition>(input, _options);

            // Assert
            var podVehicleBonus = Assert.IsType<PodVehicleBonusCondition>(result);
            Assert.Equal(2, podVehicleBonus.BonusValue);
            Assert.Equal(PodVehicle.ElevatorCars, podVehicleBonus.PodVehicle);
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
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<BonusCondition>(input, _options));
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
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<BonusCondition>(input, _options));
        }

        [Fact]
        public void Serialize_WhenCharacterBonus_WritesExpectedJson()
        {
            // Arrange
            var bonus = new CharacterBonusCondition
            {
                BonusValue = 3,
                Character = Character.Scott,
                Location = BoardLocation.IndianOcean
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
        public void Serialize_WhenCharacterBonusWithoutLocation_WritesExpectedJson()
        {
            // Arrange
            var bonus = new CharacterBonusCondition
            {
                BonusValue = 5,
                Character = Character.Alan
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
        public void Serialize_WhenThunderbirdBonus_WritesExpectedJson()
        {
            // Arrange
            var bonus = new ThunderbirdBonusCondition
            {
                BonusValue = 4,
                Thunderbird = ThunderbirdMachine.Thunderbird2,
                Location = BoardLocation.NorthPacific
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
        public void Serialize_WhenThunderbirdBonusWithoutLocation_WritesExpectedJson()
        {
            // Arrange
            var bonus = new ThunderbirdBonusCondition
            {
                BonusValue = 6,
                Thunderbird = ThunderbirdMachine.Thunderbird3
            };

            // Act
            var result = SerializeBonusToJson(bonus);

            // Assert
            Assert.Equal("thunderbirdBonus", result.GetProperty("type").GetString());
            Assert.Equal(6, result.GetProperty("bonusValue").GetInt32());
            Assert.Equal("thunderbird3", result.GetProperty("thunderbird").GetString());
            Assert.False(result.TryGetProperty("location", out _), "Location should not be present in JSON");
        }

        [Fact]
        public void Serialize_WhenPodVehicleBonus_WritesExpectedJson()
        {
            // Arrange
            var bonus = new PodVehicleBonusCondition
            {
                BonusValue = 1,
                PodVehicle = PodVehicle.ElevatorCars,
                Location = BoardLocation.SouthAtlantic
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
        public void Serialize_WhenPodVehicleBonusWithoutLocation_WritesExpectedJson()
        {
            // Arrange
            var bonus = new PodVehicleBonusCondition
            {
                BonusValue = 3,
                PodVehicle = PodVehicle.Mole
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
            var bonus = new FakeBonus
            {
                BonusValue = 10,
                FakeProperty = "Invalid"
            };

            // Act & Assert
            Assert.Throws<JsonException>(() => JsonSerializer.Serialize<BonusCondition>(bonus, _options));
        }

        [Fact]
        public void RoundTrip_WhenCharacterBonus_SerializesAndDeserializesCorrectly()
        {
            // Arrange
            var characterBonus = new CharacterBonusCondition
            {
                BonusValue = 2,
                Character = Character.Virgil,
                Location = BoardLocation.IndianOcean
            };

            // Act
            var json = JsonSerializer.Serialize<BonusCondition>(characterBonus, _options);
            var result = JsonSerializer.Deserialize<BonusCondition>(json, _options);

            // Assert
            var bonus = Assert.IsType<CharacterBonusCondition>(result);
            Assert.Equal(characterBonus.BonusValue, bonus.BonusValue);
            Assert.Equal(characterBonus.Character, bonus.Character);
            Assert.Equal(characterBonus.Location, bonus.Location);
        }

        [Fact]
        public void RoundTrip_WhenThunderbirdBonus_SerializesAndDeserializesCorrectly()
        {
            // Arrange
            var thunderbirdBonus = new ThunderbirdBonusCondition
            {
                BonusValue = 3,
                Thunderbird = ThunderbirdMachine.Thunderbird1,
                Location = BoardLocation.NorthAtlantic
            };

            // Act
            var json = JsonSerializer.Serialize<BonusCondition>(thunderbirdBonus, _options);
            var result = JsonSerializer.Deserialize<BonusCondition>(json, _options);

            // Assert
            var bonus = Assert.IsType<ThunderbirdBonusCondition>(result);
            Assert.Equal(thunderbirdBonus.BonusValue, bonus.BonusValue);
            Assert.Equal(thunderbirdBonus.Thunderbird, bonus.Thunderbird);
            Assert.Equal(thunderbirdBonus.Location, bonus.Location);
        }

        [Fact]
        public void RoundTrip_WhenPodVehicleBonus_SerializesAndDeserializesCorrectly()
        {
            // Arrange
            var podVehicleBonus = new PodVehicleBonusCondition
            {
                BonusValue = 4,
                PodVehicle = PodVehicle.Mole,
                Location = BoardLocation.SouthPacific
            };

            // Act
            var json = JsonSerializer.Serialize<BonusCondition>(podVehicleBonus, _options);
            var result = JsonSerializer.Deserialize<BonusCondition>(json, _options);

            // Assert
            var bonus = Assert.IsType<PodVehicleBonusCondition>(result);
            Assert.Equal(podVehicleBonus.BonusValue, bonus.BonusValue);
            Assert.Equal(podVehicleBonus.PodVehicle, bonus.PodVehicle);
            Assert.Equal(podVehicleBonus.Location, bonus.Location);
        }

        private JsonElement SerializeBonusToJson(BonusCondition bonus)
        {
            var json = JsonSerializer.Serialize(bonus, _options);

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }

        private class FakeBonus : BonusCondition
        {
            public string? FakeProperty { get; set; }
        }
    }
}
