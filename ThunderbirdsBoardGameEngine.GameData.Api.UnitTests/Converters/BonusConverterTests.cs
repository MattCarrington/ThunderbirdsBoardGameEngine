using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.GameData.Api.Converters;
using ThunderbirdsBoardGameEngine.GameData.Api.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Enums;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.UnitTests.Converters
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
        public void Deserialize_CharacterBonusJson_ReturnsExpectedCharacterBonus()
        {
            // Arrange
            var input = """
            {
                "type": "CharacterBonus",
                "bonusValue": 2,
                "character": "Virgil"
            }
            """;

            // Act
            var result = JsonSerializer.Deserialize<Bonus>(input, _options);

            // Assert
            var characterBonus = Assert.IsType<CharacterBonus>(result);
            Assert.Equal(2, characterBonus.BonusValue);
            Assert.Equal(Character.Virgil, characterBonus.Character);
        }

        [Fact]
        public void Deserialize_ThunderbirdBonusJson_ReturnsExpectedThunderbirdBonus()
        {
            // Arrange
            var input = """
            {
                "type": "ThunderbirdBonus",
                "bonusValue": 2,
                "thunderbird": "Thunderbird4"
            }
            """;

            // Act
            var result = JsonSerializer.Deserialize<Bonus>(input, _options);

            // Assert
            var thunderbirdBonus = Assert.IsType<ThunderbirdBonus>(result);
            Assert.Equal(2, thunderbirdBonus.BonusValue);
            Assert.Equal(Thunderbird.Thunderbird4, thunderbirdBonus.Thunderbird);
        }

        [Fact]        
        public void Deserialize_PodVehicleBonusJson_ReturnsExpectedPodVehicleBonus()
        {
            // Arrange
            var input = """
            {
                "type": "PodVehicleBonus",
                "bonusValue": 2,
                "podVehicle": "ElevatorCars"
            }
            """;            

            // Act
            var result = JsonSerializer.Deserialize<Bonus>(input, _options);

            // Assert
            var podVehicleBonus = Assert.IsType<PodVehicleBonus>(result);
            Assert.Equal(2, podVehicleBonus.BonusValue);
            Assert.Equal(PodVehicle.ElevatorCars, podVehicleBonus.PodVehicle);
        }

        [Fact]
        public void Deserialize_UnknownType_ThrowsJsonException()
        {
            // Arrange
            var input = """
            {
                "type": "UnknownBonus",
                "bonusValue": 2,
                "unknown": "Unknown"
            }
            """;

            // Act & Assert
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Bonus>(input, _options));
        }

        [Fact]
        public void Deserilize_MissingType_ThrowsJsonException()
        {
            // Arrange
            var input = """
            {
                "bonusValue": 2,
                "unknown": "Unknown"
            }
            """;

            // Act & Assert
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Bonus>(input, _options));
        }
    }
}
