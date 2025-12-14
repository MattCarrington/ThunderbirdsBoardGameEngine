using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.UnitTests.Entities
{
    public class BonusConditionsTests
    {
        [Theory]
        [MemberData(nameof(BonusConditions))]
        public void Constructor_WhenCalledWithInvalidBonusValueBelow_ThrowsArgumentOutOfRangeException(
            Func<int, BonusCondition> factory)
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => factory(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => factory(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => factory(int.MinValue));
        }

        [Theory]
        [MemberData(nameof(BonusConditions))]
        public void Constructor_WhenCalledWithValidBonusValue_CreatesInstance(
            Func<int, BonusCondition> createBonusCondition)
        {
            // Arrange

            // Act
            var result = createBonusCondition(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.BonusValue);
        }

        [Theory]
        [MemberData(nameof(BonusConditionsWithLocation))]
        public void Constructor_WithLocation_SetsLocation(
            Func<int, BoardLocation?, BonusCondition> factory)
        {
            // Arrange

            // Act
            var bc = factory(2, BoardLocation.Europe);

            // Assert
            Assert.Equal(BoardLocation.Europe, bc.Location);
        }

        [Theory]
        [MemberData(nameof(BonusConditionsWithLocation))]
        public void Constructor_WithLocation_WhenCalledWithInvalidBonusValue_ThrowsArgumentOutOfRangeException(
            Func<int, BoardLocation?, BonusCondition> factory)
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => factory(0, BoardLocation.Europe));
        }

        public static TheoryData<Func<int, BonusCondition>> BonusConditions()
        {
            return
            [
                value => new CharacterBonusCondition(Character.John, value),
                value => new ThunderbirdBonusCondition(ThunderbirdMachine.Thunderbird1, value),
                value => new PodVehicleBonusCondition(PodVehicle.Thunderizer, value)
            ];
        }

        public static TheoryData<Func<int, BoardLocation?, BonusCondition>> BonusConditionsWithLocation()
        {
            return
            [
                (value, location) => new CharacterBonusCondition(Character.John, value, location),
                (value, location) => new ThunderbirdBonusCondition(ThunderbirdMachine.Thunderbird1, value, location),
                (value, location) => new PodVehicleBonusCondition(PodVehicle.Thunderizer, value, location),
            ];
        }
    }
}
