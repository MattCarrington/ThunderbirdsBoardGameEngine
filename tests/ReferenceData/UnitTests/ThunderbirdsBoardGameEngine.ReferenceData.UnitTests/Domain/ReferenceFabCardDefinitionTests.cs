using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Domain
{
    public class ReferenceFabCardDefinitionTests
    {
        private static CardCode ValidCardCode => new("fab-1");

        private static string ValidDisplayName => "Fab Card 1";

        [Fact]
        public void Constructor_WhenAllInputsValid_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferenceFabCardDefinition(
                code: ValidCardCode,
                displayName: ValidDisplayName
            );

            // Assert
            Assert.Equal(ValidCardCode, result.Code);
            Assert.Equal(ValidDisplayName, result.DisplayName);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Constructor_WhenDisplayNameInvalid_ThrowsArgumentException(string displayName)
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new ReferenceFabCardDefinition(
                code: ValidCardCode,
                displayName: displayName
            ));
        }
    }
}
