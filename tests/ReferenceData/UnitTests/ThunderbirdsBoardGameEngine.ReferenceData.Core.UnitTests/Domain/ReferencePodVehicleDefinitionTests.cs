using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Domain
{
    public class ReferencePodVehicleDefinitionTests
    {
        private static PodVehicleCode ValidPodVehicleCode => new("podVehicle");

        private static string ValidDisplayName => "Pod Vehicle";

        [Fact]
        public void Constructor_WhenAllInputsValid_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferencePodVehicleDefinition(
                code: ValidPodVehicleCode,
                displayName: ValidDisplayName
            );

            // Assert
            Assert.Equal(ValidPodVehicleCode, result.Code);
            Assert.Equal(ValidDisplayName, result.DisplayName);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Constructor_WhenDisplayNameInvalid_ThrowsArgumentException(string? displayName)
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new ReferencePodVehicleDefinition(
                code: ValidPodVehicleCode,
                displayName: displayName
            ));
        }
    }
}
