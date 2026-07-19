using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.UnitTests.Validators
{
    public class DisasterBonusLocationOverrideValidatorTests
    {
        [Fact]
        public void Validate_WhenDisasterBonusLocationNull_DoesNotThrowException()
        {
            // Arrange
            var snapshot = new ReferenceDataSnapshotBuilder()
               .WithLocation("location-1", "Location 1")
               .WithCharacter("scott", "Scott")
               .WithThunderbird("thunderbird-1", "Thunderbird 1")
               .WithPodVehicle("mole", "The Mole")
               .WithDisaster("disaster-1", "Disaster 1", "location-1",
                   ("scott", 2, null),
                   ("thunderbird-1", 2, null),
                   ("mole", 3, null))
               .Build();

            var validator = new DisasterBonusLocationOverrideValidator();

            // Act & Assert
            var exception = Record.Exception(() => validator.Validate(snapshot));
            Assert.Null(exception);
        }

        [Fact]
        public void Validate_WhenDisasterBonusLocationDifferentFromDisasterLocation_DoesNotThrowException()
        {
            // Arrange
            var snapshot = new ReferenceDataSnapshotBuilder()
               .WithLocation("location-1", "Location 1")
               .WithLocation("location-2", "Location 2")
               .WithCharacter("scott", "Scott")
               .WithThunderbird("thunderbird-1", "Thunderbird 1")
               .WithPodVehicle("mole", "The Mole")
               .WithDisaster("disaster-1", "Disaster 1", "location-1",
                   ("scott", 2, "location-2"),
                   ("thunderbird-1", 2, "location-2"),
                   ("mole", 3, null))
               .Build();
            var validator = new DisasterBonusLocationOverrideValidator();
            // Act & Assert
            var exception = Record.Exception(() => validator.Validate(snapshot));
            Assert.Null(exception);
        }

        [Fact]
        public void Validate_WhenDisasterBonusLocationMatchesDisasterLocation_ThrowsReferenceDataCompilationException()
        {
            // Arrange
            var snapshot = new ReferenceDataSnapshotBuilder()
               .WithLocation("location-1", "Location 1")
               .WithCharacter("scott", "Scott")
               .WithThunderbird("thunderbird-1", "Thunderbird 1")
               .WithPodVehicle("mole", "The Mole")
               .WithDisaster("disaster-1", "Disaster 1", "location-1",
                   ("scott", 2, "location-1"),
                   ("thunderbird-1", 2, "location-2"),
                   ("mole", 3, null))
               .Build();

            var validator = new DisasterBonusLocationOverrideValidator();

            // Act & Assert
            var exception = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
            Assert.Equal(
                "Bonus target 'scott' on disaster 'Disaster 1' explicitly specifies the disaster location 'location-1'. Leave the bonus location empty to inherit the disaster location.",
                exception.Message);
        }
    }
}
