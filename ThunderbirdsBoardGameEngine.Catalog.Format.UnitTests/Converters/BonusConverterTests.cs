using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.TestUtils;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.UnitTests.Converters
{
    public class BonusConverterTests
    {
        private readonly JsonSerializerOptions _options = JsonDefaults.DisasterCards;

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
            var bonus = new CharacterBonusCondition(Character.Scott, 3, BoardLocation.IndianOcean);
            
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
            var bonus = new CharacterBonusCondition(Character.Alan, 5);
            
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
            var bonus = new ThunderbirdBonusCondition(ThunderbirdMachine.Thunderbird2, 4, BoardLocation.NorthPacific);
            
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
            var bonus = new ThunderbirdBonusCondition(ThunderbirdMachine.Thunderbird3, 3);

            // Act
            var result = SerializeBonusToJson(bonus);

            // Assert
            Assert.Equal("thunderbirdBonus", result.GetProperty("type").GetString());
            Assert.Equal(3, result.GetProperty("bonusValue").GetInt32());
            Assert.Equal("thunderbird3", result.GetProperty("thunderbird").GetString());
            Assert.False(result.TryGetProperty("location", out _), "Location should not be present in JSON");
        }

        [Fact]
        public void Serialize_WhenPodVehicleBonus_WritesExpectedJson()
        {
            // Arrange
            var bonus = new PodVehicleBonusCondition(PodVehicle.ElevatorCars, 1, BoardLocation.SouthAtlantic);  
            
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
            var bonus = new PodVehicleBonusCondition(PodVehicle.Mole, 3);
            
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
            var bonus = new FakeBonus(10)
            {
                FakeProperty = "Invalid"
            };

            // Act & Assert
            Assert.Throws<JsonException>(() => JsonSerializer.Serialize<BonusCondition>(bonus, _options));
        }

        [Fact]
        public void RoundTrip_WhenCharacterBonus_SerializesAndDeserializesCorrectly()
        {
            // Arrange
            var characterBonus = new CharacterBonusCondition(Character.Virgil, 2, BoardLocation.IndianOcean);

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
            var thunderbirdBonus = new ThunderbirdBonusCondition(ThunderbirdMachine.Thunderbird1, 3, BoardLocation.NorthAtlantic);
            
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
            var podVehicleBonus = new PodVehicleBonusCondition(PodVehicle.Mole, 4, BoardLocation.SouthPacific);
            
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
            public FakeBonus(int value) : base(value, null)
            {                
            }

            public string? FakeProperty { get; set; }
        }
    }
}
