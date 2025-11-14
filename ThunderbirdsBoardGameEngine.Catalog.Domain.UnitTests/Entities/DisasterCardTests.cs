using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.UnitTests.Entities
{

    public class DisasterCardTests
    {
        [Theory]
        [ClassData(typeof(WhitespaceStringData))]
        public void Constructor_WhenNameIsInvalid_ThrowsArgumentException(string name)
        {
            // Arrange
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new DisasterCard(1, name, "slug", 9, BoardLocation.Europe, RescueType.Air, ValidBonusCondition(), ValidRewardOption()));
        }

        [Fact]
        public void Constructor_WhenNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DisasterCard(1, null!, "slug", 9, BoardLocation.Europe, RescueType.Air, ValidBonusCondition(), ValidRewardOption()));
        }

        [Theory]
        [InlineData("      mission name")]
        [InlineData("mission name      ")]
        [InlineData("      mission name      ")]
        public void Constructor_WhenNameHasWhitespace_TrimsWhitespace(string nameWithWhitespace)
        {
            // Arrange
            var expectedName = "mission name";

            // Act
            var card = new DisasterCard(1, nameWithWhitespace, "slug", 9, BoardLocation.Europe, RescueType.Air,
                ValidBonusCondition(), ValidRewardOption());

            // Assert
            Assert.Equal(expectedName, card.Name);
        }

        [Theory]
        [InlineData("The Cham-Cham")]
        [InlineData("Rockquake!")]
        [InlineData("Target: Tiger One")]
        [InlineData("They Call Him Mr. X")]
        public void Constructor_NamesWithPunctuation_IsAccepted(string title)
        {
            // Arrange

            // Act
            var card = new DisasterCard(1, title, "slug", 9, BoardLocation.Europe, RescueType.Air,
                ValidBonusCondition(), ValidRewardOption());

            // Assert
            Assert.Equal(title, card.Name);
        }

        [Theory]
        [InlineData("Bad\u0001Name")]
        [InlineData("Bad\tName")]
        [InlineData("Bad\nName")]
        public void Ctor_Name_ControlChars_Throws(string name)
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new DisasterCard(1, name, "slug", 9, BoardLocation.Europe, RescueType.Air, ValidBonusCondition(), ValidRewardOption()));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void Constructor_WhenDifficultyNumberInvalid_ThrowsArgumentOutOfRangeException(int difficultyNumber)
        {
            // Arrange
            
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new DisasterCard(
                1, "Test", "slug", difficultyNumber, BoardLocation.Europe, RescueType.Air, ValidBonusCondition(), ValidRewardOption()));
        }

        [Fact]
        public void Constructor_WhenBonusConditionsIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DisasterCard(1, "Test", "slug", 9, BoardLocation.Europe, RescueType.Air, null!, ValidRewardOption()));
        }

        [Fact]
        public void Constructor_WhenRewardOptionsIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DisasterCard(1, "Test", "slug", 9, BoardLocation.Europe, RescueType.Air, ValidBonusCondition(), null!));
        }

        [Fact]
        public void Constructor_WhenEmptyBonusConditions_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new DisasterCard(1, "Test", "slug", 9, BoardLocation.Europe, RescueType.Air, [], ValidRewardOption()));
        }

        [Fact]
        public void Constructor_WhenEmptyRewardOptions_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new DisasterCard(1, "Test", "slug", 9, BoardLocation.Europe, RescueType.Air, ValidBonusCondition(), []));
        }

        [Fact]
        public void Constructor_WhenBonusConditionsHasNull_ThrowsArgumentNullException()
        {
            // Arrange
            var conditions = ValidBonusCondition();
            conditions.Add(null!);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new DisasterCard(1, "Test", "slug", 9, BoardLocation.Europe, RescueType.Air, conditions, ValidRewardOption()));
        }

        [Fact]
        public void Constructor_WhenRewardOptionsHasNull_ThrowsArgumentNullException()
        {
            // Arrange
            var rewards = ValidRewardOption();
            rewards.Add(null!);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new DisasterCard(1, "Test", "slug", 9, BoardLocation.Europe, RescueType.Air, ValidBonusCondition(), rewards));
        }

        [Fact]
        public void Constructor_WhenCharacterNotUnique_ThrowsArgumentException()
        {
            // Arrange
            var conditions = ValidBonusCondition();
            conditions.Add(new CharacterBonusCondition(Character.Virgil, 1, BoardLocation.NorthPacific)); // Duplicate character used

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new DisasterCard(1, "Test", "slug", 9, BoardLocation.Europe, RescueType.Air, conditions, ValidRewardOption()));
        }

        [Fact]
        public void Constructor_WhenCharacterUnique_IsCreated()
        {
            // Arrange
            var conditions = ValidBonusCondition();
            conditions.Add(new CharacterBonusCondition(Character.Scott, 1, BoardLocation.NorthPacific)); // Different character used

            // Act
            var card = new DisasterCard(1, "Test", "slug", 9, BoardLocation.Europe, RescueType.Air, conditions, ValidRewardOption());

            // Assert
            Assert.NotNull(card);
            Assert.Equal(4, card.BonusConditions.Count);
            Assert.Contains(card.BonusConditions, bc => bc is CharacterBonusCondition cbc && cbc.Character == Character.Scott);
            Assert.Contains(card.BonusConditions, bc => bc is CharacterBonusCondition cbc && cbc.Character == Character.Virgil);
        }

        [Fact]
        public void Constructor_WhenThunderbirdNotUnique_ThrowsArgumentException()
        {
            // Arrange
            var conditions = ValidBonusCondition();
            conditions.Add(new ThunderbirdBonusCondition(ThunderbirdMachine.Thunderbird2, 1, BoardLocation.NorthPacific)); // Duplicate Thunderbird used

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new DisasterCard(1, "Test", "slug", 9, BoardLocation.Europe, RescueType.Air, conditions, ValidRewardOption()));
        }

        [Fact]
        public void Constructor_WhenThunderbirdUnique_IsCreated()
        {
            // Arrange
            var conditions = ValidBonusCondition();
            conditions.Add(new ThunderbirdBonusCondition(ThunderbirdMachine.Thunderbird3, 1, BoardLocation.NorthPacific)); // Different Thunderbird used
            // Act
            var card = new DisasterCard(1, "Test", "slug", 9, BoardLocation.Europe, RescueType.Air, conditions, ValidRewardOption());
            // Assert
            Assert.NotNull(card);
            Assert.Equal(4, card.BonusConditions.Count);
            Assert.Contains(card.BonusConditions, bc => bc is ThunderbirdBonusCondition tbc && tbc.Thunderbird == ThunderbirdMachine.Thunderbird3);
            Assert.Contains(card.BonusConditions, bc => bc is ThunderbirdBonusCondition tbc && tbc.Thunderbird == ThunderbirdMachine.Thunderbird2);
        }

        [Fact]
        public void Constructor_WhenPodVehicleNotUnique_ThrowsArgumentException()
        {
            // Arrange
            var conditions = ValidBonusCondition();
            conditions.Add(new PodVehicleBonusCondition(PodVehicle.Firefly, 1)); // Duplicate Pod Vehicle used
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new DisasterCard(1, "Test", "slug", 9, BoardLocation.Europe, RescueType.Air, conditions, ValidRewardOption()));
        }

        [Fact]
        public void Constructor_WhenPodVehicleUnique_IsCreated()
        {
            // Arrange
            var conditions = ValidBonusCondition();
            conditions.Add(new PodVehicleBonusCondition(PodVehicle.Domo, 1)); // Different Pod Vehicle used
            // Act
            var card = new DisasterCard(1, "Test", "slug", 9, BoardLocation.Europe, RescueType.Air, conditions, ValidRewardOption());
            // Assert
            Assert.NotNull(card);
            Assert.Equal(4, card.BonusConditions.Count);
            Assert.Contains(card.BonusConditions, bc => bc is PodVehicleBonusCondition pbc && pbc.PodVehicle == PodVehicle.Domo);
            Assert.Contains(card.BonusConditions, bc => bc is PodVehicleBonusCondition pbc && pbc.PodVehicle == PodVehicle.Firefly);
        }

        [Fact]
        public void Constructor_WhenBonusConditionIsOfUnknownType_ThrowsArgumentException()
        {
            // Arrange
            var conditions = ValidBonusCondition();
            conditions.Add(new UnknownBonusCondition(2)); // Unknown bonus condition type

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new DisasterCard(1, "Test", "slug", 9, BoardLocation.Europe, RescueType.Air, conditions, ValidRewardOption()));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void Constructor_WhenCalledWithValidParameters_CreatesInstance(int difficultyNumber)
        {
            // Arrange
            
            // Act
            var card = new DisasterCard(1, "Rescue Mission", "slug", difficultyNumber, BoardLocation.NorthAmerica, RescueType.Sea,
                ValidBonusCondition(), ValidRewardOption());

            // Assert
            Assert.NotNull(card);
            Assert.Equal(1, card.Id);
            Assert.Equal("Rescue Mission", card.Name);
            Assert.Equal(difficultyNumber, card.DifficultyNumber);
            Assert.Equal(BoardLocation.NorthAmerica, card.Location);
            Assert.Equal(RescueType.Sea, card.RescueType);
            Assert.NotEmpty(card.BonusConditions);
            Assert.NotEmpty(card.RewardOptions);
        }

        [Theory]
        [InlineData("sun-probe", "sun-probe")]
        [InlineData("Sun Probe", "sun-probe")]
        [InlineData("  SUN_PROBE  ", "sun-probe")]
        [InlineData("tb2", "tb2")]
        [InlineData("Thunderbird 2", "thunderbird-2")]
        public void Constructor_WhenCodeIsValid_CodeNormalized(string input, string expected)
        {
            // Arrange

            // Act
            var card = new DisasterCard(1, "Test", input, 5, BoardLocation.Europe, RescueType.Air,
                ValidBonusCondition(), ValidRewardOption());

            // Assert
            Assert.NotNull(card);
            Assert.Equal(expected, card.Code);
        }

        [Theory]
        [ClassData(typeof(WhitespaceStringData))]
        public void Constructor_WhenCodeWhiteSpace_ThrowsArgumentException(string? code)
        {
            // Arrange

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new DisasterCard(1, "Test", code!, 5, BoardLocation.Europe, RescueType.Air,
                ValidBonusCondition(), ValidRewardOption()));
            Assert.Contains("code", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Constructor_WhenCodeIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new DisasterCard(1, "Test", null!, 5, BoardLocation.Europe, RescueType.Air,
                ValidBonusCondition(), ValidRewardOption()));

            Assert.Contains("code", exception.ParamName!, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("-bad-start")]   // leading hyphen
        [InlineData("bad-end-")]     // trailing hyphen
        [InlineData("bad--middle")]  // double hyphen
        [InlineData("bad$name")]     // illegal char
        [InlineData("über-cool")]    // non-ascii that survives after normalization (shouldn't)
        [InlineData("Élite Rescue")]
        public void Constructor_WhenCodeIsInvalid_ThrowsArgumentException(string bad)
        {
            // Arrange

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new DisasterCard(1, "Test", bad, 5, BoardLocation.Europe, RescueType.Air,
                ValidBonusCondition(), ValidRewardOption()));

            Assert.Contains("Invalid code format", exception.Message);
        }

        [Theory]
        [InlineData("Bad\u0001Name")]
        [InlineData("Bad\tName")]
        [InlineData("Bad\nName")]
        public void Constructor_WhenCodeContainsControlCharacters_ThrowsArgumentException(string code)
        {
            // Arrange

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new DisasterCard(1, "Test", code, 5, BoardLocation.Europe, RescueType.Air, 
                ValidBonusCondition(), ValidRewardOption()));
            Assert.Contains("Invalid code format", exception.Message);
        }

        private static List<BonusCondition> ValidBonusCondition()
        {
            return 
            [
                new CharacterBonusCondition(Character.Virgil, 2),
                new ThunderbirdBonusCondition(ThunderbirdMachine.Thunderbird2, 1, BoardLocation.IndianOcean),
                new PodVehicleBonusCondition(PodVehicle.Firefly, 3)
            ];
        }

        private static List<RewardOption> ValidRewardOption()
        {
            return 
            [
                RewardOption.PlayerChoice(),
                RewardOption.SpecifiedToken(BonusToken.Intelligence),
            ];
        }

        private class UnknownBonusCondition : BonusCondition
        {
            public UnknownBonusCondition(int bonusValue, BoardLocation? location = null)
                : base(bonusValue, location)
            {
            }
        }
    }
}
