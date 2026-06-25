using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.UnitTests.Domain
{
    public class ReferenceCharacterDefinitionTests
    {
        private static CharacterCode ValidCharacterCode => new("character");

        private static string ValidDisplayName => "Character Name";

        private static ReferenceCharacterRescueBonus ValidCharacterBonus => new(RescueType.Air, value: 2);

        [Fact]
        public void Constructor_WhenAllInputsValid_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferenceCharacterDefinition(
                code: ValidCharacterCode,
                displayName: ValidDisplayName,
                rescueBonus: ValidCharacterBonus
            );

            Assert.Equal(ValidCharacterCode, result.Code);
            Assert.Equal(ValidDisplayName, result.DisplayName);
            Assert.Equal(ValidCharacterBonus, result.RescueBonus);
        }

        [Fact]
        public void Constructor_WhenCharacterBonusNull_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferenceCharacterDefinition(
                code: ValidCharacterCode,
                displayName: ValidDisplayName,
                rescueBonus: null
            );

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.RescueBonus);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Constructor_WhenDisplayNameInvalid_ThrowsArgumentException(string? displayName)
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new ReferenceCharacterDefinition(
                code: ValidCharacterCode,
                displayName: displayName,
                rescueBonus: null
            ));
        }
    }
}
