using ThunderbirdsBoardGameEngine.Api.Mappers.V1;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Format.Enums;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.UnitTests.Mappers.V1
{
    public class DisasterCardMappingExtensionsTests
    {
        [Fact]
        public void MapBonusDto_WhenCharacterBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var characterBonusCondition = new CharacterBonusCondition(Character.Virgil, 2);

            // Act
            var result = characterBonusCondition.ToDto();

            // Assert
            var expectedDescription = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(characterBonusCondition.Character), characterBonusCondition.BonusValue);

            Assert.Equal(expectedDescription, result.Description);
        }

        [Fact]
        public void MapBonusConditionDto_WhenThunderbirdsBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var thunderbirdBonusCondition = new ThunderbirdBonusCondition(ThunderbirdMachine.Thunderbird4, 3);

            // Act
            var result = thunderbirdBonusCondition.ToDto();

            // Assert
            var expectedDescription = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(thunderbirdBonusCondition.Thunderbird), thunderbirdBonusCondition.BonusValue);

            Assert.Equal(expectedDescription, result.Description);
        }

        [Fact]
        public void MapBonusConditionDto_WhenPodVehicleBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var podVehicleBonusCondition = new PodVehicleBonusCondition(PodVehicle.LaserCutter, 2);

            // Act
            var result = podVehicleBonusCondition.ToDto();

            // Assert
            var expectedDescription = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(podVehicleBonusCondition.PodVehicle), podVehicleBonusCondition.BonusValue);

            Assert.Equal(expectedDescription, result.Description);
        }

        [Fact]
        public void MapBonusConditionDto_WhenUnknownBonusCondition_ThrowsInvalidOperationException()
        {
            // Arrange
            var unknownBonusCondition = new UnknownBonusCondition();

            // Act & Assert
            var ex = Assert.Throws<ApplicationValidationException>(() =>
                unknownBonusCondition.ToDto());

            Assert.Equal("Unknown bonus condition type", ex.Message);
        }

        [Fact]
        public void Map_WhenBonusHasLocation_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var characterBonusCondition = new CharacterBonusCondition(Character.Gordon, 2, BoardLocation.Venus);

            // Act
            var result = characterBonusCondition.ToDto();

            // Assert
            var expectedDescription = GetExpectedDescription(
                EnumDisplayHelper.GetDisplayName(characterBonusCondition.Character),
                characterBonusCondition.BonusValue,
                characterBonusCondition.Location);

            Assert.Equal(expectedDescription, result.Description); // This should include "in Venus" in the description
        }

        [Fact]
        public void Map_WhenBonusHasGeoStationaryOrbitLocation_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var characterBonusCondition = new CharacterBonusCondition(Character.LadyPenelope, 3, BoardLocation.GeoStationaryOrbit);

            // Act
            var result = characterBonusCondition.ToDto();

            // Assert
            var expectedDescription = GetExpectedDescription(
                EnumDisplayHelper.GetDisplayName(characterBonusCondition.Character),
                characterBonusCondition.BonusValue,
                characterBonusCondition.Location);

            Assert.Equal(expectedDescription, result.Description); // This should include "on Thunderbird 5" in the description
        }

        [Fact]
        public void MapRewardDto_WhenUserChoiceToken_ShouldCorrectlyMapRewardDto()
        {
            // Arrange
            var rewardOption = RewardOption.PlayerChoice();

            // Act
            var result = rewardOption.ToDto();

            // Assert            
            Assert.Equal("Player Choice", result.DisplayName);
        }

        [Fact]
        public void MapRewardDto_WhenSpecifiedToken_ShouldCorrectlyMapRewardDto()
        {
            // Arrange
            var rewardToken = BonusToken.Technology;

            var rewardOption = RewardOption.SpecifiedToken(rewardToken);

            // Act
            var result = rewardOption.ToDto();

            // Assert            
            Assert.Equal(rewardToken.ToString(), result.DisplayName);
        }

        [Fact]
        public void MapDisasterCardDto_WhenDisasterCard_ShouldCorrectlyMapDisasterCardDto()
        {
            // Arrange
            var disasterCard = new DisasterCardBuilder().Build();

            // Act
            var result = disasterCard.ToDto();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DisasterCardDto>(result);
            Assert.Equal(disasterCard.Id, result.Id);
            Assert.Equal(disasterCard.Name, result.Name);
            Assert.Equal(EnumDisplayHelper.GetDisplayName(disasterCard.Location), result.Location);
            Assert.Equal(disasterCard.RescueType.ToString(), result.RescueType);
            Assert.Equal(disasterCard.DifficultyNumber, result.DifficultyNumber);
        }

        [Fact]
        public void MapDisasterCardDto_WhenMultiplePodVehicleBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var mobileCraneBonusCondition = new PodVehicleBonusCondition(PodVehicle.MobileCrane, 2);

            var thunderizerBonusCondition = new PodVehicleBonusCondition(PodVehicle.Thunderizer, 3);

            var domoBonusCondition = new PodVehicleBonusCondition(PodVehicle.Domo, 1);

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(mobileCraneBonusCondition)
                .WithBonusCondition(thunderizerBonusCondition)
                .WithBonusCondition(domoBonusCondition)
                .Build();

            // Act
            var result = disasterCard.ToDto();

            // Assert
            Assert.Equal(3, result.BonusConditions.Count);

            var expectedDescriptionMobileCrane = GetExpectedDescription(
                EnumDisplayHelper.GetDisplayName(mobileCraneBonusCondition.PodVehicle),
                mobileCraneBonusCondition.BonusValue);

            var expectedDescriptionThunderizer = GetExpectedDescription(
                EnumDisplayHelper.GetDisplayName(thunderizerBonusCondition.PodVehicle),
                thunderizerBonusCondition.BonusValue);

            var expectedDescriptionDomo = GetExpectedDescription(
                EnumDisplayHelper.GetDisplayName(domoBonusCondition.PodVehicle),
                domoBonusCondition.BonusValue);

            Assert.Contains(result.BonusConditions, b => b.Description == expectedDescriptionMobileCrane);
            Assert.Contains(result.BonusConditions, b => b.Description == expectedDescriptionThunderizer);
            Assert.Contains(result.BonusConditions, b => b.Description == expectedDescriptionDomo);
        }

        [Fact]
        public void Map_WhenDifferentBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var characterBonusCondition = new CharacterBonusCondition(Character.Gordon, 2);

            var thunderbirdBonusCondition = new ThunderbirdBonusCondition(ThunderbirdMachine.Thunderbird4, 3);

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(characterBonusCondition)
                .WithBonusCondition(thunderbirdBonusCondition)
                .Build();

            // Act
            var result = disasterCard.ToDto();

            // Assert
            Assert.Equal(2, result.BonusConditions.Count);

            var expectedDescriptionCharacter = GetExpectedDescription(
                EnumDisplayHelper.GetDisplayName(characterBonusCondition.Character),
                characterBonusCondition.BonusValue);

            var expectedDescriptionThunderbird = GetExpectedDescription(
                EnumDisplayHelper.GetDisplayName(thunderbirdBonusCondition.Thunderbird),
                thunderbirdBonusCondition.BonusValue);

            Assert.Contains(result.BonusConditions, b => b.Description == expectedDescriptionCharacter);
            Assert.Contains(result.BonusConditions, b => b.Description == expectedDescriptionThunderbird);
        }

        [Fact]
        public void Map_WhenSpecifiedAndUserChoiceRewards_ShouldCorrectlyMapRewardDtos()
        {
            // Arrange
            var disasterCard = new DisasterCardBuilder()
                .WithUserChoiceRewardOption()
                .WithSpecifiedReward(BonusToken.Technology)
                .Build();

            // Act
            var result = disasterCard.ToDto();

            // Assert
            Assert.Equal(2, result.Rewards.Count);
            Assert.Contains(result.Rewards, r => r.DisplayName == "Player Choice");
            Assert.Contains(result.Rewards, r => r.DisplayName == BonusToken.Technology.ToString());
        }

        [Fact]
        public void Map_WhenMultipleRewards_ShouldCorrectlyMapRewardDtos()
        {
            // Arrange
            var disasterCard = new DisasterCardBuilder()
                .WithSpecifiedReward(BonusToken.Technology)
                .WithSpecifiedReward(BonusToken.Teamwork)
                .Build();

            // Act
            var result = disasterCard.ToDto();

            // Assert
            Assert.Equal(2, result.Rewards.Count);
            Assert.Contains(result.Rewards, r => r.DisplayName == BonusToken.Technology.ToString());
            Assert.Contains(result.Rewards, r => r.DisplayName == BonusToken.Teamwork.ToString());
        }

        [Fact]
        public void ToDto_WhenMultipleDisasterCards_ShouldCorrectlyMapList()
        {
            // Arrange
            var builder = new DisasterCardBuilder();
            var id = 0;

            var disasterCards = new List<DisasterCard>
            {
                builder.WithId(id++).WithSpecifiedReward(BonusToken.Determination).WithUserChoiceRewardOption().Build(),
                builder.WithId(id++).WithSpecifiedReward(BonusToken.Intelligence).Build(),
                builder.WithId(id).WithUserChoiceRewardOption().Build()
            };

            // Act
            var result = disasterCards.ToDto();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(disasterCards.Count, result.Count);
        }

        private static string GetExpectedDescription(string bonusName, int bonusValue, BoardLocation? bonusLocation = null)
        {
            var description = $"{bonusName} (+{bonusValue})";

            if (bonusLocation.HasValue)
            {
                var locationText = bonusLocation == BoardLocation.GeoStationaryOrbit
                    ? "on Thunderbird 5"
                    : $"in {EnumDisplayHelper.GetDisplayName(bonusLocation.Value)}";

                description += $" (if {locationText})";
            }

            return description;
        }

        private class UnknownBonusCondition : BonusCondition
        {
            public UnknownBonusCondition() : base(1, null)
            {
            }
        }
    }
}
