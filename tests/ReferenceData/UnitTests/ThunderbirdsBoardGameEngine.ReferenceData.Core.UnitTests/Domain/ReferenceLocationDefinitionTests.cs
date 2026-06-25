using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Domain
{
    public class ReferenceLocationDefinitionTests
    {
        private static LocationCode ValidLocationCode => new("location");

        private static string ValidDisplayName => "Display Name";

        [Fact]
        public void Constructor_WhenAllInputsValid_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferenceLocationDefinition(
                code: ValidLocationCode,
                displayName: ValidDisplayName,
                domain: MovementDomain.Earth
            );

            // Assert
            Assert.Equal(ValidLocationCode, result.Code);
            Assert.Equal(ValidDisplayName, result.DisplayName);
            Assert.Equal(MovementDomain.Earth, result.Domain);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Constructor_WhenDisplayNameInvalid_ThrowsArgumentException(string? displayName)
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new ReferenceLocationDefinition(
                code: ValidLocationCode,
                displayName: displayName,
                domain: MovementDomain.Earth
            ));
        }
    }
}
