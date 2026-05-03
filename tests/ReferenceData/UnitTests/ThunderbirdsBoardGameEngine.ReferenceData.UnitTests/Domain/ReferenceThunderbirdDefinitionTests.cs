using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Domain
{
    public class ReferenceThunderbirdDefinitionTests
    {
        private static ThunderbirdCode ValidThunderbirdCode => new("thunderbird");

        private static string ValidDisplayName => "Thunderbird 1";

        [Fact]
        public void Constructor_WhenAllInputsValid_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferenceThunderbirdDefinition(
                code: ValidThunderbirdCode,
                displayName: ValidDisplayName
            );

            // Assert
            Assert.Equal(ValidThunderbirdCode, result.Code);
            Assert.Equal(ValidDisplayName, result.DisplayName);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Constructor_WhenDisplayNameInvalid_ThrowsArgumentException(string displayName)
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new ReferenceThunderbirdDefinition(
                code: ValidThunderbirdCode,
                displayName: displayName
            ));
        }
    }
}
