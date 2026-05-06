using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation.Validators;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.UnitTests.Compilation.Validators
{
    public class EntityUniquenessValidatorTests
    {
        [Fact]
        public void Validate_WhenAllEntitiesUnique_DoesNotThrow()
        {
            // Arrange
            var validator = new EntityUniquenessValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithCharacter("character-1", "Character 1")
                .WithThunderbird("thunderbird-1", "Thunderbird 1")
                .WithPodVehicle("pod-1", "Pod 1")
                .Build();

            // Act & Assert
            validator.Validate(snapshot);
        }

        [Fact]
        public void Validate_WhenDuplicateLocationCodes_Throws()
        {
            // Arrange
            var validator = new EntityUniquenessValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("duplicate-code", "Location 1")
                .WithLocation("duplicate-code", "Location 2")
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("duplicate-code", ex.Message);
            Assert.Contains("location codes", ex.Message);
        }

        [Fact]
        public void Validate_WhenDuplicateLocationNames_Throws()
        {
            // Arrange
            var validator = new EntityUniquenessValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Duplicate Name")
                .WithLocation("location-2", "Duplicate Name")
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("Duplicate Name", ex.Message);
            Assert.Contains("location names", ex.Message);
        }

        [Fact]
        public void Validate_WhenDuplicateCharacterCodes_Throws()
        {
            // Arrange
            var validator = new EntityUniquenessValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithCharacter("duplicate-code", "Character 1")
                .WithCharacter("duplicate-code", "Character 2")
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("duplicate-code", ex.Message);
            Assert.Contains("character codes", ex.Message);
        }

        [Fact]
        public void Validate_WhenDuplicateCharacterNames_Throws()
        {
            // Arrange
            var validator = new EntityUniquenessValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithCharacter("char-1", "Duplicate Name")
                .WithCharacter("char-2", "Duplicate Name")
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("Duplicate Name", ex.Message);
            Assert.Contains("character names", ex.Message);
        }

        [Fact]
        public void Validate_WhenDuplicateThunderbirdCodes_Throws()
        {
            // Arrange
            var validator = new EntityUniquenessValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithThunderbird("duplicate-code", "Thunderbird 1")
                .WithThunderbird("duplicate-code", "Thunderbird 2")
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("duplicate-code", ex.Message);
            Assert.Contains("thunderbird codes", ex.Message);
        }

        [Fact]
        public void Validate_WhenDuplicateThunderbirdNames_Throws()
        {
            // Arrange
            var validator = new EntityUniquenessValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithThunderbird("tb-1", "Duplicate Name")
                .WithThunderbird("tb-2", "Duplicate Name")
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("Duplicate Name", ex.Message);
            Assert.Contains("thunderbird names", ex.Message);
        }

        [Fact]
        public void Validate_WhenDuplicatePodVehicleCodes_Throws()
        {
            // Arrange
            var validator = new EntityUniquenessValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithPodVehicle("duplicate-code", "Pod 1")
                .WithPodVehicle("duplicate-code", "Pod 2")
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("duplicate-code", ex.Message);
            Assert.Contains("pod vehicle codes", ex.Message);
        }

        [Fact]
        public void Validate_WhenDuplicatePodVehicleNames_Throws()
        {
            // Arrange
            var validator = new EntityUniquenessValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithPodVehicle("pod-1", "Duplicate Name")
                .WithPodVehicle("pod-2", "Duplicate Name")
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("Duplicate Name", ex.Message);
            Assert.Contains("pod vehicle names", ex.Message);
        }

        [Fact]
        public void Validate_WhenDuplicateDisasterCodes_Throws()
        {
            // Arrange
            var validator = new EntityUniquenessValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithCharacter("character-1", "Character 1")
                .WithDisaster("duplicate-code", "Disaster 1", "location-1", ("character-1", 1, null))
                .WithDisaster("duplicate-code", "Disaster 2", "location-1", ("character-1", 1, null))
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("duplicate-code", ex.Message);
            Assert.Contains("disaster codes", ex.Message);
        }

        [Fact]
        public void Validate_WhenDuplicateDisasterNames_Throws()
        {
            // Arrange
            var validator = new EntityUniquenessValidator();
            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithLocation("location-1", "Location 1")
                .WithCharacter("character-1", "Character 1")
                .WithDisaster("disaster-1", "Duplicate Name", "location-1", ("character-1", 1, null))
                .WithDisaster("disaster-2", "Duplicate Name", "location-1", ("character-1", 1, null))
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() =>
                validator.Validate(snapshot));
            Assert.Contains("Duplicate Name", ex.Message);
            Assert.Contains("disaster names", ex.Message);
        }
    }
}