using ThunderbirdsBoardGameEngine.Api.Mappers.Catalog.V1;
using ThunderbirdsBoardGameEngine.Api.Presentation;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Mappers.Catalog.V1
{
    public class DisasterCardMappingExtensionsTests
    {
        [Fact]
        public void ToDto_WhenCharacterBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var characterBonusCondition = new CharacterBonusCondition(Character.Virgil, 2);

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(characterBonusCondition)
                .Build();

            // Act
            var result = disasterCard.ToDto();

            // Assert
            var expectedDescription = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(characterBonusCondition.Character), characterBonusCondition.BonusValue);

            var bonusDto = Assert.Single(result.BonusConditions);
            Assert.IsType<BonusConditionDto>(bonusDto);
            Assert.Equal(expectedDescription, bonusDto.Description);
            Assert.Equal("character:virgil", bonusDto.Key);
        }

        [Fact]
        public void ToDto_WhenThunderbirdsBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var thunderbirdBonusCondition = new ThunderbirdBonusCondition(ThunderbirdMachine.Thunderbird4, 3);

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(thunderbirdBonusCondition)
                .Build();

            // Act
            var result = disasterCard.ToDto();

            // Assert
            var expectedDescription = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(thunderbirdBonusCondition.Thunderbird), thunderbirdBonusCondition.BonusValue);

            var bonusDto = Assert.Single(result.BonusConditions);
            Assert.IsType<BonusConditionDto>(bonusDto);
            Assert.Equal(expectedDescription, bonusDto.Description);
            Assert.Equal("thunderbird:thunderbird4", bonusDto.Key);
        }

        [Fact]
        public void ToDto_WhenPodVehicleBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var podVehicleBonusCondition = new PodVehicleBonusCondition(PodVehicle.LaserCutter, 2);

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(podVehicleBonusCondition)
                .Build();

            // Act
            var result = disasterCard.ToDto();

            // Assert
            var expectedDescription = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(podVehicleBonusCondition.PodVehicle), podVehicleBonusCondition.BonusValue);

            var bonusDto = Assert.Single(result.BonusConditions);
            Assert.IsType<BonusConditionDto>(bonusDto);
            Assert.Equal(expectedDescription, bonusDto.Description);
            Assert.Equal("podvehicle:lasercutter", bonusDto.Key);
        }

        [Fact]
        public void ToDto_WhenBonusHasLocation_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var characterBonusCondition = new CharacterBonusCondition(Character.Gordon, 2, BoardLocation.Venus);

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(characterBonusCondition)
                .Build();

            // Act
            var result = disasterCard.ToDto();

            // Assert
            var expectedDescription = GetExpectedDescription(
                EnumDisplayHelper.GetDisplayName(characterBonusCondition.Character),
                characterBonusCondition.BonusValue,
                characterBonusCondition.Location);

            var bonusDto = Assert.Single(result.BonusConditions);
            Assert.IsType<BonusConditionDto>(bonusDto);
            Assert.Equal(expectedDescription, bonusDto.Description); // This should include "in Venus" in the description
            Assert.Equal("character:gordon", bonusDto.Key);
        }

        [Fact]
        public void ToDto_WhenBonusHasGeoStationaryOrbitLocation_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var characterBonusCondition = new CharacterBonusCondition(Character.LadyPenelope, 3, BoardLocation.GeoStationaryOrbit);

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(characterBonusCondition)
                .Build();

            // Act
            var result = disasterCard.ToDto();

            // Assert
            var expectedDescription = GetExpectedDescription(
                EnumDisplayHelper.GetDisplayName(characterBonusCondition.Character),
                characterBonusCondition.BonusValue,
                characterBonusCondition.Location);

            var bonusDto = Assert.Single(result.BonusConditions);
            Assert.IsType<BonusConditionDto>(bonusDto);
            Assert.Equal(expectedDescription, bonusDto.Description); // This should include "on Thunderbird 5" in the description
            Assert.Equal("character:ladypenelope", bonusDto.Key);
        }

        [Fact]
        public void ToDto_WhenUserChoiceToken_ShouldCorrectlyMapRewardDto()
        {
            // Arrange
            var disasterCard = new DisasterCardBuilder()
                .WithUserChoiceRewardOption()
                .Build();

            // Act
            var result = disasterCard.ToDto();

            // Assert  
            var reward = Assert.Single(result.Rewards);
            Assert.IsType<RewardDto>(reward);
            Assert.Equal("Player Choice", reward.DisplayName);
        }

        [Fact]
        public void ToDto_WhenSpecifiedToken_ShouldCorrectlyMapRewardDto()
        {
            // Arrange
            var rewardToken = BonusToken.Technology;

            var disasterCard = new DisasterCardBuilder()
                .WithSpecifiedReward(rewardToken)
                .Build();

            // Act
            var result = disasterCard.ToDto();

            // Assert            
            var reward = Assert.Single(result.Rewards);
            Assert.IsType<RewardDto>(reward);
            Assert.Equal(rewardToken.ToString(), reward.DisplayName);
        }

        [Fact]
        public void ToDto_WhenDisasterCard_ShouldCorrectlyMapDisasterCardDto()
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
            Assert.Equal(disasterCard.Code.ToString(), result.Code);
        }

        [Fact]
        public void ToDto_WhenMultiplePodVehicleBonus_ShouldCorrectlyMapBonusDto()
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
        public void To_WhenDifferentBonus_ShouldCorrectlyMapBonusDto()
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
        public void ToDto_WhenSpecifiedAndUserChoiceRewards_ShouldCorrectlyMapRewardDtos()
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
        public void ToDto_WhenMultipleRewards_ShouldCorrectlyMapRewardDtos()
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
    }
}
