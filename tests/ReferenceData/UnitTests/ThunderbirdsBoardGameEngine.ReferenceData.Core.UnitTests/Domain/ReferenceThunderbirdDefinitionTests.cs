using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.UnitTests.Domain
{
    public class ReferenceThunderbirdDefinitionTests
    {
        private static ThunderbirdCode ValidThunderbirdCode => new("thunderbird");

        private static string ValidDisplayName => "Thunderbird 1";

        private static MovementDomain ValidDomain => MovementDomain.Earth;

        private static int ValidTopSpeed => 3;

        private static int ValidCharacterCapacity => 2;

        [Fact]
        public void Constructor_WhenAllInputsValid_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferenceThunderbirdDefinition(
                code: ValidThunderbirdCode,
                displayName: ValidDisplayName,
                domain: ValidDomain,
                topSpeed: ValidTopSpeed,
                characterCapacity: ValidCharacterCapacity);

            // Assert
            Assert.Equal(ValidThunderbirdCode, result.Code);
            Assert.Equal(ValidDisplayName, result.DisplayName);
            Assert.Equal(ValidDomain, result.Domain);
            Assert.Equal(ValidTopSpeed, result.TopSpeed);
            Assert.Equal(ValidCharacterCapacity, result.CharacterCapacity);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Constructor_WhenDisplayNameInvalid_ThrowsArgumentException(string? displayName)
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new ReferenceThunderbirdDefinition(
                code: ValidThunderbirdCode,
                displayName: displayName,
                domain: ValidDomain,
                topSpeed: ValidTopSpeed,
                characterCapacity: ValidCharacterCapacity));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public void Constructor_WhenTopSpeedValid_CreatesInstance(int topSpeed)
        {
            // Arrange

            // Act
            var result = new ReferenceThunderbirdDefinition(
                code: ValidThunderbirdCode,
                displayName: ValidDisplayName,
                domain: ValidDomain,
                topSpeed: topSpeed,
                characterCapacity: ValidCharacterCapacity);

            // Assert
            Assert.Equal(topSpeed, result.TopSpeed);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Constructor_WhenTopSpeedInvalid_ThrowsArgumentException(int topSpeed)
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new ReferenceThunderbirdDefinition(
                code: ValidThunderbirdCode,
                displayName: ValidDisplayName,
                domain: ValidDomain,
                topSpeed: topSpeed,
                characterCapacity: ValidCharacterCapacity));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void Constructor_WhenCharacterCapacityValid(int characterCapacity)
        {
            // Arrange

            // Act
            var result = new ReferenceThunderbirdDefinition(
                code: ValidThunderbirdCode,
                displayName: ValidDisplayName,
                domain: ValidDomain,
                topSpeed: ValidTopSpeed,
                characterCapacity: characterCapacity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Constructor_WhenCharacterCapacityInvalid_ThrowsArgumentOutOfRangeException(int characterCapacity)
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReferenceThunderbirdDefinition(
                code: ValidThunderbirdCode,
                displayName: ValidDisplayName,
                domain: ValidDomain,
                topSpeed: ValidTopSpeed,
                characterCapacity: characterCapacity));
        }
    }
}
