using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Domain
{
    public class ReferenceDisasterDefinitionTests
    {
        private static CardCode ValidCardCode => new("card");

        private static string ValidDisplayName => "Display Name";

        private static int ValidDifficultyNumber => 1;

        private static LocationCode ValidLocationCode => new("location");

        private static RescueType ValidRescueType => RescueType.Air;

        private static List<ReferenceDisasterBonus> ValidBonuses
        {
            get
            {
                return new()
                {
                    new ReferenceDisasterBonus(new DisasterBonusKey("bonus1"), 1, null)
                };
            }
        }

        private static List<ReferenceDisasterReward> ValidRewards
        {
            get
            {
                return new()
                {
                    new ReferenceDisasterReward.PlayerChoice(),
                    new ReferenceDisasterReward.SpecificToken(BonusToken.Technology)
                };
            }
        }

        [Fact]
        public void Constructor_WhenAllInputsValid_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferenceDisasterDefinition(
                code: ValidCardCode,
                displayName: ValidDisplayName,
                difficultyNumber: ValidDifficultyNumber,
                location: ValidLocationCode,
                rescueType: ValidRescueType,
                bonuses: ValidBonuses,
                rewards: ValidRewards
            );

            // Assert
            Assert.Equal(ValidCardCode, result.Code);
            Assert.Equal(ValidDisplayName, result.DisplayName);
            Assert.Equal(ValidDifficultyNumber, result.DifficultyNumber);
            Assert.Equal(ValidLocationCode, result.Location);
            Assert.Equal(ValidRescueType, result.RescueType);
            Assert.Equal(ValidBonuses, result.Bonuses);
            Assert.Equal(ValidRewards, result.Rewards);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Constructor_WhenDisplayNameInvalid_ThrowsArgumentException(string? displayName)
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new ReferenceDisasterDefinition(
                code: ValidCardCode,
                displayName: displayName,
                difficultyNumber: ValidDifficultyNumber,
                location: ValidLocationCode,
                rescueType: ValidRescueType,
                bonuses: ValidBonuses,
                rewards: ValidRewards
            ));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Constructor_WhenDifficultyNumberNotPositive_ThrowsArgumentOutOfRangeException(int difficultyNumber)
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReferenceDisasterDefinition(
                code: ValidCardCode,
                displayName: ValidDisplayName,
                difficultyNumber: difficultyNumber,
                location: ValidLocationCode,
                rescueType: ValidRescueType,
                bonuses: ValidBonuses,
                rewards: ValidRewards
            ));
        }

        [Fact]
        public void Constructor_WhenBonusesNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ReferenceDisasterDefinition(
                code: ValidCardCode,
                displayName: ValidDisplayName,
                difficultyNumber: ValidDifficultyNumber,
                location: ValidLocationCode,
                rescueType: ValidRescueType,
                bonuses: null,
                rewards: ValidRewards
            ));
        }

        [Fact]
        public void Constructor_WhenBonusesEmpty_ThrowsArgumentOutOfRangeException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReferenceDisasterDefinition(
                code: ValidCardCode,
                displayName: ValidDisplayName,
                difficultyNumber: ValidDifficultyNumber,
                location: ValidLocationCode,
                rescueType: ValidRescueType,
                bonuses: [],
                rewards: ValidRewards
            ));
        }

        [Fact]
        public void Constructor_WhenRewardsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ReferenceDisasterDefinition(
                code: ValidCardCode,
                displayName: ValidDisplayName,
                difficultyNumber: ValidDifficultyNumber,
                location: ValidLocationCode,
                rescueType: ValidRescueType,
                bonuses: ValidBonuses,
                rewards: null
            ));
        }

        [Fact]
        public void Constructor_WhenRewardsEmpty_ThrowsArgumentOutOfRangeException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReferenceDisasterDefinition(
                code: ValidCardCode,
                displayName: ValidDisplayName,
                difficultyNumber: ValidDifficultyNumber,
                location: ValidLocationCode,
                rescueType: ValidRescueType,
                bonuses: ValidBonuses,
                rewards: []
            ));
        }
    }
}
