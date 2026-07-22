using ThunderbirdsBoardGameEngine.Api.Exceptions;
using ThunderbirdsBoardGameEngine.Api.Mappers.Rules.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Mappers.Rules.V1
{
    public class RescueMappingExtensionsTests
    {
        private static readonly CardCode ValidDisasterCardCode = new CardCode("card-code-123");

        [Fact]
        public void ToQuery_ValidDto_ReturnsExpectedQuery()
        {
            // Arrange
            var dto = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = ["character:alan", "thunderbird:thunderbird4", "podvehicle.domo"],
                PerformingCharacterKey = "alan",
                PlayedFabCardKeys = ["fab1", "fab2"],
                ActiveEventCardKeys = ["event1", "event2"]
            };

            // Act
            var result = dto.ToQuery(ValidDisasterCardCode.ToString());

            // Assert
            Assert.Equal(ValidDisasterCardCode, result.DisasterCardCode);
            Assert.Equal(dto.PresentDisasterBonusKeys.Select(k => new DisasterBonusKey(k)), result.PresentDisasterBonusKeys);
            Assert.Equal(new CharacterCode("alan"), result.PerformingCharacter);
            Assert.Equal(dto.PlayedFabCardKeys.Select(c => new CardCode(c)), result.PlayedFabCardCodes);
            Assert.Equal(dto.ActiveEventCardKeys.Select(c => new CardCode(c)), result.ActiveEventCardCodes);
        }

        [Fact]
        public void ToQuery_OptionalKeysMissing_ReturnsExpectedQuery()
        {
            // Arrange
            var dto = new CalculateRescueTargetRequestDto
            {
                PerformingCharacterKey = "alan"
                // PresentDisasterBonusKeys, PlayedFabCards and ActiveEventCards are not set, should default to empty lists
            };

            // Act
            var result = dto.ToQuery(ValidDisasterCardCode.ToString());

            // Assert
            Assert.Equal(ValidDisasterCardCode, result.DisasterCardCode);
            Assert.Empty(result.PresentDisasterBonusKeys);
            Assert.Equal(new CharacterCode("alan"), result.PerformingCharacter);
            Assert.Empty(result.PlayedFabCardCodes);
            Assert.Empty(result.ActiveEventCardCodes);
        }

        [Fact]
        public void ToQuery_PresentDisasterBonusKeysEmpty_ReturnsExpectedQuery()
        {
            // Arrange
            var dto = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = [],
                PerformingCharacterKey = "alan"
            };

            // Act
            var result = dto.ToQuery(ValidDisasterCardCode.ToString());

            // Assert
            Assert.Equal(ValidDisasterCardCode, result.DisasterCardCode);
            Assert.Empty(result.PresentDisasterBonusKeys);
            Assert.Equal(new CharacterCode("alan"), result.PerformingCharacter);
        }

        [Fact]
        public void ToQuery_PlayedFabCardKeysEmpty_ReturnsExpectedQuery()
        {
            // Arrange
            var dto = new CalculateRescueTargetRequestDto
            {
                PerformingCharacterKey = "alan",
                PlayedFabCardKeys = []
            };

            // Act
            var result = dto.ToQuery(ValidDisasterCardCode.ToString());

            // Assert
            Assert.Equal(ValidDisasterCardCode, result.DisasterCardCode);
            Assert.Equal(new CharacterCode("alan"), result.PerformingCharacter);
            Assert.Empty(result.PlayedFabCardCodes);
        }

        [Fact]
        public void ToQuery_ActiveEventCardKeysEmpty_ReturnsExpectedQuery()
        {
            // Arrange
            var dto = new CalculateRescueTargetRequestDto
            {
                PerformingCharacterKey = "alan",
                ActiveEventCardKeys = []
            };

            // Act
            var result = dto.ToQuery(ValidDisasterCardCode.ToString());

            // Assert
            Assert.Equal(ValidDisasterCardCode, result.DisasterCardCode);
            Assert.Equal(new CharacterCode("alan"), result.PerformingCharacter);
            Assert.Empty(result.ActiveEventCardCodes);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void ToQuery_PerformingCharacterKeyNullOrEmpty_ThrowsBadRequestException(string? character)
        {
            // Arrange
            var dto = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = ["character:alan", "thunderbird:thunderbird4", "podvehicle.domo"],
                PerformingCharacterKey = character
            };

            // Act & Assert
            Assert.Throws<BadRequestException>(() => dto.ToQuery(ValidDisasterCardCode.ToString()));
        }

        [Fact]
        public void ToQuery_PresentDisasterBonusKeysNull_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = null,
                PerformingCharacterKey = "alan"
            };

            // Act & Assert
            Assert.Throws<BadRequestException>(() => dto.ToQuery(ValidDisasterCardCode.ToString()));
        }

        [Fact]
        public void ToQuery_PlayedFabCardKeysNull_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new CalculateRescueTargetRequestDto
            {
                PerformingCharacterKey = "alan",
                PlayedFabCardKeys = null
            };

            // Act & Assert
            Assert.Throws<BadRequestException>(() => dto.ToQuery(ValidDisasterCardCode.ToString()));
        }

        [Fact]
        public void ToQuery_ActiveEventCardKeysNull_ThrowsBadRequestException()
        {
            // Arrange
            var dto = new CalculateRescueTargetRequestDto
            {
                PerformingCharacterKey = "alan",
                ActiveEventCardKeys = null
            };

            // Act & Assert
            Assert.Throws<BadRequestException>(() => dto.ToQuery(ValidDisasterCardCode.ToString()));
        }

        [Fact]
        public void ToDto_DisasterCardAppliedBonuses_ReturnsExpectedDto()
        {
            // Arrange
            var response = new CalculateRescueTargetResponse
            (
                TargetNumber: 12,
                TotalBonus: 3,
                AppliedBonuses:
                [
                    new AppliedRescueModifier
                    {
                        Key = "character:alan",
                        Value = 2,
                        SourceType = SourceType.DisasterCard
                    },
                    new AppliedRescueModifier
                    {
                        Key = "thunderbird:thunderbird4",
                        Value = 1,
                        SourceType = SourceType.DisasterCard
                    }
                ]
            );

            // Act
            var result = response.ToDto();

            // Assert
            Assert.Equal(response.TargetNumber, result.TargetNumber);
            Assert.Equal(response.TotalBonus, result.TotalBonus);
            Assert.Equal(response.AppliedBonuses.Count, result.AppliedDisasterBonuses.Count);

            var expectedBonus = new List<AppliedDisasterBonusDto>
            {
                new() { BonusKey = "character:alan", BonusValue = 2, SourceType = "disaster-card" },
                new() { BonusKey = "thunderbird:thunderbird4", BonusValue = 1, SourceType = "disaster-card" }
            };

            Assert.All(result.AppliedDisasterBonuses, expected =>
                Assert.Contains(expected, expectedBonus));
        }

        [Fact]
        public void ToDto_CharacterAbilityAppliedBonus_ReturnsExpectedDto()
        {
            // Arrange
            var response = new CalculateRescueTargetResponse
            (
                TargetNumber: 12,
                TotalBonus: 3,
                AppliedBonuses:
                [
                    new AppliedRescueModifier
                    {
                        Key = "alan",
                        Value = 2,
                        SourceType = SourceType.CharacterAbility
                    }
                ]
            );

            // Act
            var result = response.ToDto();

            // Assert
            Assert.Equal(response.TargetNumber, result.TargetNumber);
            Assert.Equal(response.TotalBonus, result.TotalBonus);

            var bonus = Assert.Single(result.AppliedDisasterBonuses);
            Assert.Equal("alan", bonus.BonusKey);
            Assert.Equal(2, bonus.BonusValue);
            Assert.Equal("character-ability", bonus.SourceType);
        }

        [Fact]
        public void ToDto_FabCardAppliedBonus_ReturnsExpectedDto()
        {
            // Arrange
            var response = new CalculateRescueTargetResponse
            (
                TargetNumber: 12,
                TotalBonus: 3,
                AppliedBonuses:
                [
                    new AppliedRescueModifier
                    {
                        Key = "fab1",
                        Value = 2,
                        SourceType = SourceType.FabCard
                    }
                ]
            );

            // Act
            var result = response.ToDto();

            // Assert
            Assert.Equal(response.TargetNumber, result.TargetNumber);
            Assert.Equal(response.TotalBonus, result.TotalBonus);

            var bonus = Assert.Single(result.AppliedDisasterBonuses);
            Assert.Equal("fab1", bonus.BonusKey);
            Assert.Equal(2, bonus.BonusValue);
            Assert.Equal("fab-card", bonus.SourceType);
        }

        [Fact]
        public void ToDto_EventCardAppliedBonus_ReturnsExpectedDto()
        {
            // Arrange
            var response = new CalculateRescueTargetResponse
            (
                TargetNumber: 12,
                TotalBonus: 3,
                AppliedBonuses:
                [
                    new AppliedRescueModifier
                    {
                        Key = "event1",
                        Value = 2,
                        SourceType = SourceType.EventCard
                    }
                ]
            );

            // Act
            var result = response.ToDto();

            // Assert
            Assert.Equal(response.TargetNumber, result.TargetNumber);
            Assert.Equal(response.TotalBonus, result.TotalBonus);

            var bonus = Assert.Single(result.AppliedDisasterBonuses);
            Assert.Equal("event1", bonus.BonusKey);
            Assert.Equal(2, bonus.BonusValue);
            Assert.Equal("event-card", bonus.SourceType);
        }
    }
}
