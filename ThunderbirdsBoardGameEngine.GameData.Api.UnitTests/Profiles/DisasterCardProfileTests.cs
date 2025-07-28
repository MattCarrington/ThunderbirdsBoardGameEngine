using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos;
using ThunderbirdsBoardGameEngine.GameData.Api.Profiles;
using ThunderbirdsBoardGameEngine.GameData.Api.TestHelpers.Builders;
using ThunderbirdsBoardGameEngine.Serialization.Enums;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.UnitTests.Profiles
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
            var characterBonus = new CharacterBonus()
            {
                Character = Character.Virgil,
                BonusValue = 2
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonus(characterBonus)
                .Build();

            

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            var bonus = Assert.Single(result.Bonuses);
            
            var expectedDescription = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(characterBonus.Character), characterBonus.BonusValue);

            Assert.Equal(expectedDescription, bonus.Description);
        }

        [Fact]
        public void Map_WhenThunderbirdsBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var thunderbirdBonus = new ThunderbirdBonus()
            {
                Thunderbird = ThunderbirdMachine.Thunderbird4,
                BonusValue = 3
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonus(thunderbirdBonus)
                .Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            var bonus = Assert.Single(result.Bonuses);

            var expectedDescription = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(thunderbirdBonus.Thunderbird), thunderbirdBonus.BonusValue);

            Assert.Equal(expectedDescription, bonus.Description);
        }

        [Fact]
        public void Map_WhenPodVehicleBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var podVehicleBonus = new PodVehicleBonus()
            {
                PodVehicle = PodVehicle.LaserCutter,
                BonusValue = 4
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonus(podVehicleBonus)
                .Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            var bonus = Assert.Single(result.Bonuses);
            
            var expectedDescription = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(podVehicleBonus.PodVehicle), podVehicleBonus.BonusValue);

            Assert.Equal(expectedDescription, bonus.Description);
        }

        [Fact]
        public void Map_WhenMultiplePodVehicleBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var mobileCraneBonus = new PodVehicleBonus()
            {
                PodVehicle = PodVehicle.MobileCrane,
                BonusValue = 2
            };

            var thunderizerBonus = new PodVehicleBonus()
            {
                PodVehicle = PodVehicle.Thunderizer,
                BonusValue = 3
            };

            var domoBonus = new PodVehicleBonus()
            {
                PodVehicle = PodVehicle.Domo,
                BonusValue = 4
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonus(mobileCraneBonus)
                .WithBonus(thunderizerBonus)
                .WithBonus(domoBonus)
                .Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            Assert.Equal(3, result.Bonuses.Count);
            
            var expectedDescriptionMobileCrane = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(mobileCraneBonus.PodVehicle), mobileCraneBonus.BonusValue);
            var expectedDescriptionThunderizer = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(thunderizerBonus.PodVehicle), thunderizerBonus.BonusValue);
            var expectedDescriptionDomo = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(domoBonus.PodVehicle), domoBonus.BonusValue);

            Assert.Contains(result.Bonuses, b => b.Description == expectedDescriptionMobileCrane);
            Assert.Contains(result.Bonuses, b => b.Description == expectedDescriptionThunderizer);
            Assert.Contains(result.Bonuses, b => b.Description == expectedDescriptionDomo);
        }

        [Fact]
        public void Map_WhenDifferentBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var characterBonus = new CharacterBonus()
            {
                Character = Character.Gordon,
                BonusValue = 2
            };

            var thunderbirdBonus = new ThunderbirdBonus()
            {
                Thunderbird = ThunderbirdMachine.Thunderbird4,
                BonusValue = 3
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonus(characterBonus)
                .WithBonus(thunderbirdBonus)
                .Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            Assert.Equal(2, result.Bonuses.Count);

            var expectedDescriptionCharacter = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(characterBonus.Character), characterBonus.BonusValue);
            var expectedDescriptionThunderbird = GetExpectedDescription(EnumDisplayHelper.GetDisplayName(thunderbirdBonus.Thunderbird), thunderbirdBonus.BonusValue);

            Assert.Contains(result.Bonuses, b => b.Description == expectedDescriptionCharacter);
            Assert.Contains(result.Bonuses, b => b.Description == expectedDescriptionThunderbird);
        }

        [Fact]
        public void Map_WhenBonusHasLocation_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var characterBonus = new CharacterBonus()
            {
                Character = Character.Gordon,
                BonusValue = 2,
                Location = BoardLocation.Venus
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonus(characterBonus)
                .Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            var bonus = Assert.Single(result.Bonuses);

            var expectedDescription = GetExpectedDescription(
                EnumDisplayHelper.GetDisplayName(characterBonus.Character),
                characterBonus.BonusValue,
                characterBonus.Location);

            Assert.Equal(expectedDescription, bonus.Description); // This should include "in Venus" in the description
        }

        [Fact]
        public void Map_WhenBonusHasGeoStationaryOrbitLocation_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var characterBonus = new CharacterBonus()
            {
                Character = Character.LadyPenelope,
                BonusValue = 3,
                Location = BoardLocation.GeoStationaryOrbit
            };
            var disasterCard = new DisasterCardBuilder()
                .WithBonus(characterBonus)
                .Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            var bonus = Assert.Single(result.Bonuses);

            var expectedDescription = GetExpectedDescription(
                EnumDisplayHelper.GetDisplayName(characterBonus.Character),
                characterBonus.BonusValue,
                characterBonus.Location); 

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
        public void Map_WhenUnknownBonusType_ThrowsInvalidOperationException()
        {
            // Arrange
            var unknownBonus = new UnknownBonus();
            var disasterCard = new DisasterCardBuilder()
                .WithBonus(unknownBonus)
                .Build();

            // Act & Assert
            var ex = Assert.Throws<AutoMapperMappingException>(() =>
                _mapper.Map<DisasterCardDto>(disasterCard));

            Assert.IsType<InvalidOperationException>(ex.InnerException);
            Assert.Equal("Unknown bonus type", ex.InnerException?.Message);
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


        private class UnknownBonus : Bonus
        {
            // No extra fields needed — we just want it to be an unknown subtype.
        }
    }
}
