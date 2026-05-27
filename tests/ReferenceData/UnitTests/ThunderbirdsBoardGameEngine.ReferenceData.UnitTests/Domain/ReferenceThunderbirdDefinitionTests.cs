using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
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

        private static MovementDomain ValidDomain => MovementDomain.Earth;

        [Fact]
        public void Constructor_WhenAllInputsValid_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferenceThunderbirdDefinition(
                code: ValidThunderbirdCode,
                displayName: ValidDisplayName,
                domain: ValidDomain
            );

            // Assert
            Assert.Equal(ValidThunderbirdCode, result.Code);
            Assert.Equal(ValidDisplayName, result.DisplayName);
            Assert.Equal(ValidDomain, result.Domain);
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
                domain: ValidDomain
            ));
        }
    }
}
