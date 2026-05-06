using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation.Validators;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.UnitTests.Compilation.Validators
{
    public class DisasterBonusSystemValidatorTests
    {
        [Fact]
        public void Validate_WhenAssetCodesUniqueAcrossTypes_DoesNotThrow()
        {
            // Arrange
            var validator = new DisasterBonusSystemValidator();

            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithCharacter("scott", "Scott")
                .WithThunderbird("thunderbird-1", "Thunderbird 1")
                .WithPodVehicle("mole", "The Mole")
                .WithDisaster("disaster-1", "Disaster 1", "location-1", ("scott", 1, null))
                .Build();

            // Act & Assert
            validator.Validate(snapshot);
        }

        [Fact]
        public void Validate_WhenAssetCodeDuplicatedAcrossTypes_Throws()
        {
            // Arrange
            var validator = new DisasterBonusSystemValidator();

            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithCharacter("duplicate-code", "Character")
                .WithThunderbird("duplicate-code", "Thunderbird")
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("duplicate-code", ex.Message);
            Assert.Contains("unique across types", ex.Message);
        }

        [Fact]
        public void Validate_WhenBonusKeyReferencesValidCharacter_DoesNotThrow()
        {
            // Arrange
            var validator = new DisasterBonusSystemValidator();

            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithCharacter("scott", "Scott")
                .WithDisaster("disaster-1", "Disaster 1", "location-1", ("scott", 2, null))
                .Build();

            // Act & Assert
            validator.Validate(snapshot);
        }

        [Fact]
        public void Validate_WhenBonusKeyReferencesValidThunderbird_DoesNotThrow()
        {
            // Arrange
            var validator = new DisasterBonusSystemValidator();

            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithThunderbird("thunderbird-1", "Thunderbird 1")
                .WithDisaster("disaster-1", "Disaster 1", "location-1", ("thunderbird-1", 2, null))
                .Build();

            // Act & Assert
            validator.Validate(snapshot);
        }

        [Fact]
        public void Validate_WhenBonusKeyReferencesValidPodVehicle_DoesNotThrow()
        {
            // Arrange
            var validator = new DisasterBonusSystemValidator();

            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithPodVehicle("mole", "The Mole")
                .WithDisaster("disaster-1", "Disaster 1", "location-1", ("mole", 2, null))
                .Build();

            // Act & Assert
            validator.Validate(snapshot);
        }

        [Fact]
        public void Validate_WhenBonusKeyReferencesInvalidAsset_Throws()
        {
            // Arrange
            var validator = new DisasterBonusSystemValidator();

            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithCharacter("scott", "Scott")
                .WithDisaster("disaster-1", "Disaster 1", "location-1", ("invalid-entity", 2, null))
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("invalid-entity", ex.Message);
            Assert.Contains("non-existent assets", ex.Message);
        }

        [Fact]
        public void Validate_WhenMultipleBonusKeysReferenceValidAssetsAcrossTypes_DoesNotThrow()
        {
            // Arrange
            var validator = new DisasterBonusSystemValidator();

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

            // Act & Assert
            validator.Validate(snapshot);
        }
    }
}