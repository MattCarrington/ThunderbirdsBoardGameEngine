using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Domain
{
    public class ReferenceEventCardDefinitionTests
    {
        private static CardCode ValidCardCode => new("event-1");

        private static string ValidDisplayName => "Event Card 1";

        [Fact]
        public void Constructor_WhenAllInputsValid_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferenceEventCardDefinition(
                code: ValidCardCode,
                displayName: ValidDisplayName
            );

            // Assert
            Assert.Equal(ValidCardCode, result.Code);
            Assert.Equal(ValidDisplayName, result.DisplayName);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Constructor_WhenDisplayNameInvalid_ThrowsArgumentException(string? displayName)
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new ReferenceEventCardDefinition(
                code: ValidCardCode,
                displayName: displayName
            ));
        }
    }
}
