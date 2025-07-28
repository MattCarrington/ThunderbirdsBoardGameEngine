using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos;
using ThunderbirdsBoardGameEngine.GameData.Api.Profiles.V1;
using ThunderbirdsBoardGameEngine.GameData.Api.TestHelpers.Builders;
using ThunderbirdsBoardGameEngine.Serialization.Enums;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.UnitTests.Profiles.V1
{
    public class DisasterCardProfileTests
    {
        private readonly IMapper _mapper;
        
        public DisasterCardProfileTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<DisasterCardProfile>(), NullLoggerFactory.Instance);
            config.AssertConfigurationIsValid();
            _mapper = new Mapper(config);
        }

        [Fact]
        public void Map_WhenDisasterCard_ShouldCorrectlyMapDisasterCardDto()
        {
            // Arrange
            var disasterCard = new DisasterCardBuilder()
                .Build();
            
            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(disasterCard.Id, result.Id);
            Assert.Equal(disasterCard.Name, result.Name);
            Assert.Equal(EnumDisplayHelper.GetDisplayName(disasterCard.Location), result.Location);
            Assert.Equal(disasterCard.RescueType.ToString(), result.RescueType);
            Assert.Equal(disasterCard.DifficultyNumber, result.DifficultyNumber);
        }

        [Fact]
        public void Map_WhenCharacterBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var characterBonusCondition = new CharacterBonusCondition()
            {
                Character = Character.Virgil,
                BonusValue = 2
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(characterBonusCondition)
                .Build();

            

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            var bonus = Assert.Single(result.BonusConditions);
            
            var expectedDescription = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(characterBonusCondition.Character), characterBonusCondition.BonusValue);

            Assert.Equal(expectedDescription, bonus.Description);
        }

        [Fact]
        public void Map_WhenThunderbirdsBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var thunderbirdBonusCondition = new ThunderbirdBonusCondition()
            {
                Thunderbird = ThunderbirdMachine.Thunderbird4,
                BonusValue = 3
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(thunderbirdBonusCondition)
                .Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            var bonus = Assert.Single(result.BonusConditions);

            var expectedDescription = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(thunderbirdBonusCondition.Thunderbird), thunderbirdBonusCondition.BonusValue);

            Assert.Equal(expectedDescription, bonus.Description);
        }

        [Fact]
        public void Map_WhenPodVehicleBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var podVehicleBonusCondition = new PodVehicleBonusCondition()
            {
                PodVehicle = PodVehicle.LaserCutter,
                BonusValue = 4
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(podVehicleBonusCondition)
                .Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            var bonus = Assert.Single(result.BonusConditions);
            
            var expectedDescription = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(podVehicleBonusCondition.PodVehicle), podVehicleBonusCondition.BonusValue);

            Assert.Equal(expectedDescription, bonus.Description);
        }

        [Fact]
        public void Map_WhenMultiplePodVehicleBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var mobileCraneBonusCondition = new PodVehicleBonusCondition()
            {
                PodVehicle = PodVehicle.MobileCrane,
                BonusValue = 2
            };

            var thunderizerBonusCondition = new PodVehicleBonusCondition()
            {
                PodVehicle = PodVehicle.Thunderizer,
                BonusValue = 3
            };

            var domoBonusCondition = new PodVehicleBonusCondition()
            {
                PodVehicle = PodVehicle.Domo,
                BonusValue = 4
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(mobileCraneBonusCondition)
                .WithBonusCondition(thunderizerBonusCondition)
                .WithBonusCondition(domoBonusCondition)
                .Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

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
            var characterBonusCondition = new CharacterBonusCondition()
            {
                Character = Character.Gordon,
                BonusValue = 2
            };

            var thunderbirdBonusCondition = new ThunderbirdBonusCondition()
            {
                Thunderbird = ThunderbirdMachine.Thunderbird4,
                BonusValue = 3
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(characterBonusCondition)
                .WithBonusCondition(thunderbirdBonusCondition)
                .Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

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
        public void Map_WhenBonusHasLocation_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var characterBonusCondition = new CharacterBonusCondition()
            {
                Character = Character.Gordon,
                BonusValue = 2,
                Location = BoardLocation.Venus
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(characterBonusCondition)
                .Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            var bonus = Assert.Single(result.BonusConditions);

            var expectedDescription = GetExpectedDescription(
                EnumDisplayHelper.GetDisplayName(characterBonusCondition.Character),
                characterBonusCondition.BonusValue,
                characterBonusCondition.Location);

            Assert.Equal(expectedDescription, bonus.Description); // This should include "in Venus" in the description
        }

        [Fact]
        public void Map_WhenBonusHasGeoStationaryOrbitLocation_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var characterBonusCondition = new CharacterBonusCondition()
            {
                Character = Character.LadyPenelope,
                BonusValue = 3,
                Location = BoardLocation.GeoStationaryOrbit
            };
            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(characterBonusCondition)
                .Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            var bonus = Assert.Single(result.BonusConditions);

            var expectedDescription = GetExpectedDescription(
                EnumDisplayHelper.GetDisplayName(characterBonusCondition.Character),
                characterBonusCondition.BonusValue,
                characterBonusCondition.Location); 

            Assert.Equal(expectedDescription, bonus.Description); // This should include "on Thunderbird 5" in the description
        }

        [Fact]
        public void Map_WhenUserChoiceToken_ShouldCorrectlyMapRewardDto()
        {
            // Arrange
            var disasterCard = new DisasterCardBuilder().WithUserChoiceRewardOption().Build();
            
            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert            
            var reward = Assert.Single(result.Rewards);
            Assert.Equal("User Choice", reward.DisplayName);
        }

        [Fact]
        public void Map_WhenSpecifiedToken_ShouldCorrectlyMapRewardDto()
        {
            // Arrange
            var rewardToken = BonusToken.Technology;

            var disasterCard = new DisasterCardBuilder().WithSpecifiedReward(rewardToken).Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert            
            var reward = Assert.Single(result.Rewards);
            Assert.Equal(rewardToken.ToString(), reward.DisplayName);
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
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            Assert.Equal(2, result.Rewards.Count);
            Assert.Contains(result.Rewards, r => r.DisplayName == "User Choice");
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
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            Assert.Equal(2, result.Rewards.Count);
            Assert.Contains(result.Rewards, r => r.DisplayName == BonusToken.Technology.ToString());
            Assert.Contains(result.Rewards, r => r.DisplayName == BonusToken.Teamwork.ToString());
        }

        [Fact]
        public void Map_WhenUnknownBonusCondition_ThrowsInvalidOperationException()
        {
            // Arrange
            var unknownBonusCondition = new UnknownBonusCondition();

            var disasterCard = new DisasterCardBuilder()
                .WithBonusCondition(unknownBonusCondition)
                .Build();

            // Act & Assert
            var ex = Assert.Throws<AutoMapperMappingException>(() =>
                _mapper.Map<DisasterCardDto>(disasterCard));

            Assert.IsType<InvalidOperationException>(ex.InnerException);
            Assert.Equal("Unknown bonus condition", ex.InnerException?.Message);
        }

        [Fact]
        public void Map_WhenNonUserChoiceRewardWithoutSpecifiedToken_ThrowsInvalidOperationException()
        {
            // Arrange
            var disasterCard = new DisasterCardBuilder()
                .WithUserChoiceRewardOption()
                .Build();

            disasterCard.RewardOptions[0].IsUserChoice = false;

            // Act & Assert
            var ex = Assert.Throws<AutoMapperMappingException>(() =>
                _mapper.Map<DisasterCardDto>(disasterCard));

            Assert.IsType<InvalidOperationException>(ex.InnerException);
            Assert.Equal("SpecifiedToken must be set for non-user-choice rewards", ex.InnerException?.Message);
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
            // No extra fields needed — we just want it to be an unknown subtype.
        }
    }
}
