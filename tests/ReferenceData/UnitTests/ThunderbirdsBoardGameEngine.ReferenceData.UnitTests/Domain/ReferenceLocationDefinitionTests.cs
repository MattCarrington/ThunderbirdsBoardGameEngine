using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
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
                displayName: ValidDisplayName
            );

            // Assert
            Assert.Equal(ValidLocationCode, result.Code);
            Assert.Equal(ValidDisplayName, result.DisplayName);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Constructor_WhenDisplayNameInvalid_ThrowsArgumentException(string displayName)
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new ReferenceLocationDefinition(
                code: ValidLocationCode,
                displayName: displayName
            ));
        }
    }
}
