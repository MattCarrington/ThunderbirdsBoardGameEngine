using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.UnitTests.Compilation.Validators
{
    public class LocationReferenceValidatorTests
    {
        [Fact]
        public void Validate_WhenDisasterReferencesValidLocation_DoesNotThrow()
        {
            // Arrange
            var validator = new LocationReferenceValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("valid-location", "Valid Location")
                .WithCharacter("character-1", "Character 1")
                .WithDisaster("disaster-1", "Disaster 1", "valid-location", ("character-1", 1, null))
                .Build();

            // Act & Assert
            validator.Validate(snapshot);
        }

        [Fact]
        public void Validate_WhenDisasterReferencesInvalidLocation_Throws()
        {
            // Arrange
            var validator = new LocationReferenceValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("valid-location", "Valid Location")
                .WithCharacter("character-1", "Character 1")
                .WithDisaster("disaster-1", "Disaster 1", "invalid-location", ("character-1", 1, null))
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("invalid-location", ex.Message);
            Assert.Contains("Invalid location codes", ex.Message);
        }

        [Fact]
        public void Validate_WhenBonusReferencesValidLocation_DoesNotThrow()
        {
            // Arrange
            var validator = new LocationReferenceValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("disaster-location", "Disaster Location")
                .WithLocation("bonus-location", "Bonus Location")
                .WithCharacter("character-1", "Character 1")
                .WithDisaster("disaster-1", "Disaster 1", "disaster-location",
                    ("character-1", 2, "bonus-location"))
                .Build();

            // Act & Assert
            validator.Validate(snapshot);
        }

        [Fact]
        public void Validate_WhenBonusReferencesInvalidLocation_Throws()
        {
            // Arrange
            var validator = new LocationReferenceValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("valid-location", "Valid Location")
                .WithCharacter("character-1", "Character 1")
                .WithDisaster("disaster-1", "Disaster 1", "valid-location",
                    ("character-1", 2, "invalid-location"))
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("invalid-location", ex.Message);
            Assert.Contains("non-existent locations", ex.Message);
        }

        [Fact]
        public void Validate_WhenBonusHasNoLocation_DoesNotThrow()
        {
            // Arrange
            var validator = new LocationReferenceValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithCharacter("character-1", "Character 1")
                .WithDisaster("disaster-1", "Disaster 1", "location-1",
                    ("character-1", 2, null))  // ← No location
                .Build();

            // Act & Assert
            validator.Validate(snapshot);
        }
    }
}