using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.GameData.Api.Converters;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;
using Xunit;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;

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
                "type": "characterBonus",
                "bonusValue": 2,
                "character": "virgil"
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
                "type": "thunderbirdBonus",
                "bonusValue": 2,
                "thunderbird": "thunderbird4"
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
                "type": "podVehicleBonus",
                "bonusValue": 2,
                "podVehicle": "elevatorCars"
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
                "type": "unknownBonus",
                "bonusValue": 2,
                "unknown": "unknown"
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
                "unknown": "unknown"
            }
            """;

            // Act & Assert
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Bonus>(input, _options));
        }
    }
}
