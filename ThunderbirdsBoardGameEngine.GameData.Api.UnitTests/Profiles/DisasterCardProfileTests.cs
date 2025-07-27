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
            Assert.Equal(EnumDisplayHelper.GetDisplayName(characterBonus.Character), bonus.DisplayName);
            Assert.Equal(characterBonus.BonusValue, bonus.BonusValue);
            Assert.Null(bonus.Location); // No location specified for character bonus
        }

        [Fact]
        public void Map_WhenThunderbirdsBonus_ShouldCorrectlyMapBonusDto()
        {
            // Arrange
            var thunderbirdBonus = new ThunderbirdBonus()
            {
                Thunderbird = Thunderbird.Thunderbird4,
                BonusValue = 3
            };

            var disasterCard = new DisasterCardBuilder()
                .WithBonus(thunderbirdBonus)
                .Build();

            // Act
            var result = _mapper.Map<DisasterCardDto>(disasterCard);

            // Assert
            var bonus = Assert.Single(result.Bonuses);
            Assert.Equal(EnumDisplayHelper.GetDisplayName(thunderbirdBonus.Thunderbird), bonus.DisplayName);
            Assert.Equal(thunderbirdBonus.BonusValue, bonus.BonusValue);        
            Assert.Null(bonus.Location); // No location specified for thunderbird bonus
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
            Assert.Equal(EnumDisplayHelper.GetDisplayName(podVehicleBonus.PodVehicle), bonus.DisplayName);
            Assert.Equal(podVehicleBonus.BonusValue, bonus.BonusValue);
            Assert.Null(bonus.Location); // No location specified for pod vehicle bonus
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
            Assert.Contains(result.Bonuses, b => b.DisplayName == EnumDisplayHelper.GetDisplayName(mobileCraneBonus.PodVehicle) && b.BonusValue == mobileCraneBonus.BonusValue);
            Assert.Contains(result.Bonuses, b => b.DisplayName == EnumDisplayHelper.GetDisplayName(thunderizerBonus.PodVehicle) && b.BonusValue == thunderizerBonus.BonusValue);
            Assert.Contains(result.Bonuses, b => b.DisplayName == EnumDisplayHelper.GetDisplayName(domoBonus.PodVehicle) && b.BonusValue == domoBonus.BonusValue);
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
                Thunderbird = Thunderbird.Thunderbird4,
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
            Assert.Contains(result.Bonuses, b => b.DisplayName == EnumDisplayHelper.GetDisplayName(characterBonus.Character) && b.BonusValue == characterBonus.BonusValue);
            Assert.Contains(result.Bonuses, b => b.DisplayName == EnumDisplayHelper.GetDisplayName(thunderbirdBonus.Thunderbird) && b.BonusValue == thunderbirdBonus.BonusValue);
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
            Assert.Equal(EnumDisplayHelper.GetDisplayName(characterBonus.Location.Value), bonus.Location);
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

        private class UnknownBonus : Bonus
        {
            // No extra fields needed — we just want it to be an unknown subtype.
        }
    }
}
