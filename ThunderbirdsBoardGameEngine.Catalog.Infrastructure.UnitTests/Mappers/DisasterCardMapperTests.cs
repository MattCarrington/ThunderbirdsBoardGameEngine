using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Mappers;
using ThunderbirdsBoardGameEngine.TestUtils.Builders;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Mappers
{
    public class DisasterCardMapperTests
    {
        [Theory]
        [InlineData("SouthAmerica", "Air")]
        [InlineData("southamerica", "air")]
        [InlineData("SOUTHAMERICA", "AIR")]
        [InlineData("Southamerica", "Air")]
        [InlineData("southAmerica", "air")]
        public void Map_DisasterCard_ShouldMapDisasterCardProperties(string location, string rescueType)
        {
            // Arrange
            var dto = new DisasterCardCatalogDtoBuilder()
                .WithId(10)
                .WithName("Volcano Eruption")
                .WithDifficulty(7)
                .WithLocation(location)
                .WithRescueType(rescueType)
                .Build();

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(dto);

            // Assert
            Assert.IsType<DisasterCard>(result);
            Assert.Equal(dto.Id, result.Id);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.DifficultyNumber, result.DifficultyNumber);
            Assert.Equal(BoardLocation.SouthAmerica, result.Location);
            Assert.Equal(RescueType.Air, result.RescueType);
        }

        [Theory]
        [InlineData("LadyPenelope")]
        [InlineData("ladypenelope")]
        [InlineData("LADYPENELOPE")]
        [InlineData("ladyPenelope")]
        [InlineData("Ladypenelope")]
        public void Map_WhenCharacterBonusDto_ShouldMapToCharacterBonusCondition(string name)
        {
            // Arrange
            var bonus = new CharacterBonusCatalogDto
            {
                Character = name,
                BonusValue = 2
            };

            var dto = new DisasterCardCatalogDtoBuilder().WithBonusCondition(bonus).Build();

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(dto);

            // Assert
            Assert.IsType<DisasterCard>(result);
            var characterBonus = Assert.Single(result.BonusConditions) ;
            var character = Assert.IsType<CharacterBonusCondition>(characterBonus);
            Assert.NotNull(characterBonus);
            Assert.Equal(Character.LadyPenelope, character.Character);
            Assert.Equal(2, character.BonusValue);
            Assert.Null(character.Location);
        }

        [Theory]
        [InlineData("NorthAtlantic")]
        [InlineData("northatlantic")]
        [InlineData("NORTHATLANTIC")]
        [InlineData("Northatlantic")]
        [InlineData("northAtlantic")]
        public void Map_WhenCharacterBonusDtoWithLocation_ShouldMapToCharacterBonusConditionWithLocation(string location)
        {
            // Arrange
            var bonus = new CharacterBonusCatalogDto
            {
                Character = "Scott",
                BonusValue = 5,
                Location = location
            };

            var dto = new DisasterCardCatalogDtoBuilder().WithBonusCondition(bonus).Build();

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(dto);

            // Assert
            Assert.IsType<DisasterCard>(result);
            var characterBonus = Assert.Single(result.BonusConditions);
            var character = Assert.IsType<CharacterBonusCondition>(characterBonus);
            Assert.Equal(Character.Scott, character.Character);
            Assert.Equal(5, character.BonusValue);
            Assert.Equal(BoardLocation.NorthAtlantic, character.Location);
        }

        [Theory]
        [InlineData("Thunderbird1")]
        [InlineData("THUNDERBIRD1")]
        [InlineData("thunderbird1")]
        public void Map_WhenThunderbirdBonusDto_ShouldMapToThunderbirdBonusCondition(string name)
        {
            // Arrange
            var bonus = new ThunderbirdBonusCatalogDto
            {
                Thunderbird = name,
                BonusValue = 3
            };

            var dto = new DisasterCardCatalogDtoBuilder().WithBonusCondition(bonus).Build();
            
            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(dto);

            // Assert
            Assert.IsType<DisasterCard>(result);
            var thunderbirdBonus = Assert.Single(result.BonusConditions);
            var thunderbird = Assert.IsType<ThunderbirdBonusCondition>(thunderbirdBonus);
            Assert.Equal(ThunderbirdMachine.Thunderbird1, thunderbird.Thunderbird);
            Assert.Equal(3, thunderbird.BonusValue);
            Assert.Null(thunderbird.Location);
        }

        [Theory]
        [InlineData("IndianOcean")]
        [InlineData("indianocean")]
        [InlineData("INDIANOCEAN")]
        [InlineData("IndianOCEAN")]
        [InlineData("indianOcean")]
        public void Map_WhenThunderbirdBonusDtoWithLocation_ShouldMapToThunderbirdBonusConditionWithLocation(string location)
        {
            // Arrange
            var bonus = new ThunderbirdBonusCatalogDto
            {
                Thunderbird = "Thunderbird2",
                BonusValue = 6,
                Location = location
            };

            var dto = new DisasterCardCatalogDtoBuilder().WithBonusCondition(bonus).Build();

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(dto);

            // Assert
            Assert.IsType<DisasterCard>(result);
            var thunderbirdBonus = Assert.Single(result.BonusConditions);
            var thunderbird = Assert.IsType<ThunderbirdBonusCondition>(thunderbirdBonus);
            Assert.Equal(ThunderbirdMachine.Thunderbird2, thunderbird.Thunderbird);
            Assert.Equal(6, thunderbird.BonusValue);
            Assert.Equal(BoardLocation.IndianOcean, thunderbird.Location);
        }

        [Theory]
        [InlineData("LaserCutter")]
        [InlineData("LASERCUTTER")]
        [InlineData("lasercutter")]
        [InlineData("laserCutter")]
        [InlineData("Lasercutter")]
        public void Map_WhenPodVehicleBonusDto_ShouldMapToPodVehicleBonusCondition(string name)
        {
            // Arrange
            var bonus = new PodVehicleBonusCatalogDto
            {
                PodVehicle = name,
                BonusValue = 4
            };

            var dto = new DisasterCardCatalogDtoBuilder().WithBonusCondition(bonus).Build();
            
            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(dto);

            // Assert
            Assert.IsType<DisasterCard>(result);
            var podVehicleBonus = Assert.Single(result.BonusConditions);
            var podVehicle = Assert.IsType<PodVehicleBonusCondition>(podVehicleBonus);
            Assert.Equal(PodVehicle.LaserCutter, podVehicle.PodVehicle);
            Assert.Equal(4, podVehicle.BonusValue);
            Assert.Null(podVehicle.Location);
        }

        [Theory]
        [InlineData("Europe")]
        [InlineData("europe")]
        [InlineData("EUROPE")]
        public void Map_WhenPodVehicleBonusDtoWithLocation_ShouldMapToPodVehicleBonusConditionWithLocation(string location)
        {
            // Arrange
            var bonus = new PodVehicleBonusCatalogDto
            {
                PodVehicle = "Firefly",
                BonusValue = 7,
                Location = location
            };

            var dto = new DisasterCardCatalogDtoBuilder().WithBonusCondition(bonus).Build();

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(dto);

            // Assert
            Assert.IsType<DisasterCard>(result);
            var podVehicleBonus = Assert.Single(result.BonusConditions);
            var podVehicle = Assert.IsType<PodVehicleBonusCondition>(podVehicleBonus);
            Assert.Equal(PodVehicle.Firefly, podVehicle.PodVehicle);
            Assert.Equal(7, podVehicle.BonusValue);
            Assert.Equal(BoardLocation.Europe, podVehicle.Location);
        }

        [Fact]
        public void Map_WhenMultipleBonusConditions_ShouldMapAllConditionsCorrectly()
        {
            // Arrange
            var characterBonus = new CharacterBonusCatalogDto
            {
                Character = "Virgil",
                BonusValue = 2
            };

            var thunderbirdBonus = new ThunderbirdBonusCatalogDto
            {
                Thunderbird = "Thunderbird3",
                BonusValue = 3
            };

            var podVehicleBonus = new PodVehicleBonusCatalogDto
            {
                PodVehicle = "TransmitterTruck",
                BonusValue = 4
            };

            var dto = new DisasterCardCatalogDtoBuilder()
                .WithBonusCondition(characterBonus)
                .WithBonusCondition(thunderbirdBonus)
                .WithBonusCondition(podVehicleBonus)
                .Build();

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(dto);

            // Assert
            Assert.IsType<DisasterCard>(result);
            Assert.Equal(3, result.BonusConditions.Count);

            Assert.Contains(result.BonusConditions, b =>
                b is CharacterBonusCondition c &&
                c.Character == Character.Virgil &&
                c.BonusValue == 2 &&
                c.Location == null);

            Assert.Contains(result.BonusConditions, b =>
                b is ThunderbirdBonusCondition t &&
                t.Thunderbird == ThunderbirdMachine.Thunderbird3 &&
                t.BonusValue == 3 &&
                t.Location == null);

            Assert.Contains(result.BonusConditions, b =>
                b is PodVehicleBonusCondition p &&
                p.PodVehicle == PodVehicle.TransmitterTruck &&
                p.BonusValue == 4 &&
                p.Location == null);
        }

        [Fact]
        public void Map_WhenMultipleBonusConditionsOfSameType_ShouldMapAllConditionsCorrectly()
        {
            // Arrange
            var john = new CharacterBonusCatalogDto
            {
                Character = "John",
                BonusValue = 1
            };

            var scott = new CharacterBonusCatalogDto
            {
                Character = "Scott",
                BonusValue = 2
            };

            var dto = new DisasterCardCatalogDtoBuilder()
                .WithBonusCondition(john)
                .WithBonusCondition(scott)
                .Build();

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(dto);

            // Assert
            Assert.IsType<DisasterCard>(result);
            Assert.Equal(2, result.BonusConditions.Count);

            Assert.Contains(result.BonusConditions, b =>
                b is CharacterBonusCondition c &&
                c.Character == Character.John &&
                c.BonusValue == 1 &&
                c.Location == null);

            Assert.Contains(result.BonusConditions, b =>
                b is CharacterBonusCondition c &&
                c.Character == Character.Scott &&
                c.BonusValue == 2 &&
                c.Location == null);
        }

        [Fact]
        public void Map_WhenPlayerChoiceRewardOption_ShouldMapToPlayerChoiceReward()
        {
            // Arrange
            var dto = new DisasterCardCatalogDtoBuilder()
                .WithPlayerChoiceReward()
                .Build();

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(dto);

            // Assert
            Assert.IsType<DisasterCard>(result);
            var reward = Assert.Single(result.RewardOptions);
            Assert.IsType<RewardOption>(reward);
            Assert.True(reward.IsUserChoice);
            Assert.Null(reward.Token);
        }

        [Theory]
        [InlineData("Teamwork")]
        [InlineData("teamwork")]
        [InlineData("TEAMWORK")]
        [InlineData("TeamWork")]
        [InlineData("teamWork")]
        public void Map_WhenSpecificTokenRewardOption_ShouldMapToSpecificTokenReward(string token)
        {
            // Arrange
            var dto = new DisasterCardCatalogDtoBuilder()
                .WithSpecifiedRewardToken(token)
                .Build();

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(dto);

            // Assert
            Assert.IsType<DisasterCard>(result);
            var reward = Assert.Single(result.RewardOptions);
            Assert.IsType<RewardOption>(reward);
            Assert.False(reward.IsUserChoice);
            Assert.NotNull(reward.Token);
            Assert.Equal(BonusToken.Teamwork, reward.Token);
        }

        [Fact]
        public void Map_WhenMultipleRewardOptions_ShouldMapAllOptionsCorrectly()
        {
            // Arrange
            var dto = new DisasterCardCatalogDtoBuilder()
                .WithPlayerChoiceReward()
                .WithSpecifiedRewardToken("Intelligence")
                .Build();

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(dto);

            // Assert
            Assert.IsType<DisasterCard>(result);
            Assert.Equal(2, result.RewardOptions.Count);

            Assert.Contains(result.RewardOptions, r =>
                r.IsUserChoice &&
                r.Token == null);

            Assert.Contains(result.RewardOptions, r =>
                !r.IsUserChoice &&
                r.Token == BonusToken.Intelligence);
        }

        [Fact]
        public void Map_WhenMultipleRewardTokens_ShouldMapAllTokensCorrectly()
        {
            // Arrange
            var logistics = new TokenRewardCatalogDto
            {
                Token = "Logistics"
            };

            var determination = new TokenRewardCatalogDto
            {
                Token = "Determination"
            };

            var dto = new DisasterCardCatalogDtoBuilder()
                .WithRewardOption(logistics)
                .WithRewardOption(determination)
                .Build();

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(dto);

            // Assert
            Assert.IsType<DisasterCard>(result);
            Assert.Equal(2, result.RewardOptions.Count);

            Assert.Contains(result.RewardOptions, r =>
                !r.IsUserChoice &&
                r.Token == BonusToken.Logistics);

            Assert.Contains(result.RewardOptions, r =>
                !r.IsUserChoice &&
                r.Token == BonusToken.Determination);
        }

        [Fact]
        public void Map_WhenNullDisasterCardDto_ShouldThrowException()
        {
            // Arrange
            DisasterCardCatalogDto dto = null!;

            // Act & Assert
            AssertMapperException(dto, DisasterCardErrorCode.NullEntry);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        [InlineData("InvalidLocation")]
        public void Map_WhenLocationInvalid_ShouldThrowException(string? location)
        {
            // Arrange
            var dto = new DisasterCardCatalogDtoBuilder()
                .WithLocation(location)
                .Build();

            // Act & Assert
            AssertMapperException(dto, DisasterCardErrorCode.Unknown);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        [InlineData("Bob")]
        public void Map_WhenRescueTypeInvalid_ShouldThrowException(string? rescueType)
        {
            // Arrange
            var dto = new DisasterCardCatalogDtoBuilder()
                .WithRescueType(rescueType)
                .Build();

            // Act & Assert
            AssertMapperException(dto, DisasterCardErrorCode.Unknown);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        [InlineData("invalid")]
        public void Map_WhenCharacterInvalid_ShouldThrowException(string? character)
        {
            // Arrrange
            var bonus = new CharacterBonusCatalogDto
            {
                Character = character,
                BonusValue = 1
            };

            // Act & Assert
            AssertBonusConditionException(bonus);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        [InlineData("invalid")]
        public void Map_WhenThunderbirdMachineInvalid_ShouldThrowException(string? thunderbird)
        {
            // Arrange
            var bonus = new ThunderbirdBonusCatalogDto
            {
                Thunderbird = thunderbird,
                BonusValue = 2
            };

            // Act & Assert
            AssertBonusConditionException(bonus);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        [InlineData("invalidpod")]
        public void Map_WhenPodVehicleInvalid_ShouldThrowException(string? podVehicle)
        {
            // Arrange
            var bonus = new PodVehicleBonusCatalogDto
            {
                PodVehicle = podVehicle,
                BonusValue = 3
            };

            // Act & Assert
            AssertBonusConditionException(bonus);
        }

        [Theory]
        [InlineData("invalidlocation")]
        public void Map_BonusConditionWithInvalidLocation_ShouldThrowException(string location)
        {
            // Arrange 
            var bonus = new CharacterBonusCatalogDto
            {
                Character = "gordon",
                BonusValue = 2,
                Location = location
            };

            // Act & Assert
            AssertBonusConditionException(bonus);
        }

        [Fact]
        public void Map_WhenInvalidBonusConditionDto_ShouldThrowException()
        {
            // Arrange
            var bonus = new InvalidBonusConditionDto { BonusValue = 5 };

            // Act & Assert
            AssertBonusConditionException(bonus, DisasterCardErrorCode.UnknownBonusCondition);
        }

        [Fact]
        public void Map_WhenNullRewardOptionDto_ShouldThrowException()
        {
            // Arrange
            var dto = new DisasterCardCatalogDtoBuilder()
                .WithBonusCondition(null!)
                .Build();

            // Act & Assert
            AssertMapperException(dto, DisasterCardErrorCode.NullBonusCondition);
        }

        [Fact]
        public void Map_WhenInvalidRewardOptionDto_ShouldThrowException()
        {
            // Arrange
            var rewardOption = new InvalidRewardOptionDto();

            var dto = new DisasterCardCatalogDtoBuilder()
                .WithRewardOption(rewardOption)
                .Build();

            // Act & Assert
            AssertMapperException(dto, DisasterCardErrorCode.UnknownRewardOption);
        }

        [Fact]
        public void Map_WhenRewardOptionNullToken_ShouldThrowException()
        {
            // Arrange
            var dto = new DisasterCardCatalogDtoBuilder()
                .WithRewardOption(null!)
                .Build();

            // Act & Assert
            AssertMapperException(dto, DisasterCardErrorCode.NullRewardOption);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        [InlineData("InvalidToken")]
        public void Map_WhenRewardOptionInvalidToken_ShouldThrowException(string? token)
        {
            // Arrange
            var rewardOption = new TokenRewardCatalogDto
            {
                Token = token
            };

            var dto = new DisasterCardCatalogDtoBuilder()
                .WithRewardOption(rewardOption)
                .Build();

            // Act & Assert
            AssertMapperException(dto, DisasterCardErrorCode.Unknown);
        }

        [Fact]
        public void Map_WhenDifficultyNumberIsNegative_ShouldWrapArgumentOutOfRangeException()
        {
            // Arrange
            var dto = new DisasterCardCatalogDtoBuilder()
                .WithDifficulty(-1)
                .Build();

            // Act & Assert
            AssertOutOfRangeExceptionWrapped(dto);
        }

        [Fact]
        public void Map_WhenEmptyBonusConditions_ShouldWrapArgumentOutOfRangeException()
        {
            // Arrange
            var dto = new DisasterCardCatalogDtoBuilder()
                .WithEmptyBonuses()
                .Build();

            // Act & Assert
            AssertOutOfRangeExceptionWrapped(dto);
        }

        [Fact]
        public void Map_WhenEmptyRewardOptions_ShouldBubbleArgumentOutOfRangeException()
        {
            // Arrange
            var dto = new DisasterCardCatalogDtoBuilder()
                .WithEmptyRewards()
                .Build();

            // Act & Assert
            AssertOutOfRangeExceptionWrapped(dto);
        }

        private static DisasterCardMapper CreateMapper()
        {
            return new DisasterCardMapper();
        }

        private static void AssertMapperException(DisasterCardCatalogDto dto, DisasterCardErrorCode expectedErrorCode)
        {
            var mapper = CreateMapper();

            var exception = Assert.Throws<DisasterCardValidationException>(() => mapper.Map(dto));

            Assert.Equal(expectedErrorCode, exception.ErrorCode);
        }

        private static void AssertBonusConditionException(BonusConditionCatalogDto bonus, DisasterCardErrorCode errorCode = DisasterCardErrorCode.Unknown)
        {
            var dto = new DisasterCardCatalogDtoBuilder().WithBonusCondition(bonus).Build();

            AssertMapperException(dto, errorCode);
        }

        private static void AssertOutOfRangeExceptionWrapped(DisasterCardCatalogDto dto)
        {
            var mapper = CreateMapper();
            
            var exception = Assert.Throws<DisasterCardValidationException>(() => mapper.Map(dto));
            Assert.IsType<ArgumentOutOfRangeException>(exception.InnerException);
        }

        private sealed record InvalidBonusConditionDto : BonusConditionCatalogDto
        {
        }

        private sealed record InvalidRewardOptionDto : RewardOptionCatalogDto
        {
        }
    }
}