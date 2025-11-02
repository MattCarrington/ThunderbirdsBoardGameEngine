using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.TestUtils;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.UnitTests.Converters
{
    public class RewardOptionCatalogDtoConversionTests
    {
        private readonly JsonSerializerOptions _options = JsonDefaults.DisasterCards;

        [Fact]
        public void Deserialize_WhenPlayerChoiceJson_ReturnsExpectedRewardOptionCatalogDto()
        {
            // Arrange
            var input = """
            {
                "type": "playerChoice"               
            }
            """;

            // Act
            var result = DeserializeRewardOptionFromJson(input);

            // Assert
            Assert.IsType<PlayerChoiceRewardCatalogDto>(result);
        }

        [Fact]
        public void Deserialize_WhenTokenRewardJson_ReturnsExpectedRewardOptionCatalogDto()
        {
            // Arrange
            var input = """
            {
                "type": "token",
                "token": "logistics"
            }
            """;

            // Act
            var result = DeserializeRewardOptionFromJson(input);

            // Assert
            var tokenReward = Assert.IsType<TokenRewardCatalogDto>(result);
            Assert.Equal("logistics", tokenReward.Token);
        }

        [Fact]
        public void Deserialize_WhenUnknownType_ThrowsJsonException()
        {
            // Arrange
            var input = """
            {
                "type": "unknownType"
            }
            """;

            // Act & Assert
            Assert.Throws<JsonException>(() => DeserializeRewardOptionFromJson(input));
        }

        [Fact]
        public void Deserialize_WhenTypeMissing_ReturnsExpectedRewardOptionCatalogDto()
        {
            // Arrange
            var input = """
            {
                "token": "logistics"
            }
            """;

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => DeserializeRewardOptionFromJson(input));
        }

        [Fact]
        public void Serialize_WhenPlayerChoiceRewardOptionCatalogDto_ReturnsExpectedJson()
        {
            // Arrange
            var input = new PlayerChoiceRewardCatalogDto();

            // Act
            var result = SerializeRewardOptionToJson(input);

            // Assert
            Assert.Equal("playerChoice", result.GetProperty("type").GetString());
        }

        [Fact]
        public void Serialize_WhenTokenRewardOptionCatalogDto_ReturnsExpectedJson()
        {
            // Arrange
            var input = new TokenRewardCatalogDto
            {
                Token = "logistics"
            };

            // Act
            var result = SerializeRewardOptionToJson(input);

            // Assert
            Assert.Equal("token", result.GetProperty("type").GetString());
            Assert.Equal("logistics", result.GetProperty("token").GetString());
        }

        [Fact]
        public void Serialize_WhenUnknownRewardOptionCatalogDto_ThrowsNotSupportedException()
        {
            // Arrange
            var input = new UnknownRewardOptionCatalogDto();

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => SerializeRewardOptionToJson(input));
        }

        private RewardOptionCatalogDto DeserializeRewardOptionFromJson(string json)
        {
            return JsonSerializer.Deserialize<RewardOptionCatalogDto>(json, _options)!;
        }

        private JsonElement SerializeRewardOptionToJson(RewardOptionCatalogDto rewardOption)
        {
            var json = JsonSerializer.Serialize(rewardOption, _options);

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }

        private record UnknownRewardOptionCatalogDto : RewardOptionCatalogDto
        {
        }
    }
}
