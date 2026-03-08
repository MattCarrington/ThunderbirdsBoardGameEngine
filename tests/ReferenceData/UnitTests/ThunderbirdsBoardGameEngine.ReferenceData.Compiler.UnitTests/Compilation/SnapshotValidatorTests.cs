using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.UnitTests.Compilation
{
    public class SnapshotValidatorTests
    {
        [Fact]
        public void Validate_WhenAllDisastersUnique_DoesNotThrow()
        {
            // Arrange
            var validator = new SnapshotValidator();
            var snapshot = new ReferenceDataSnapshot(
                SchemaVersion: 1,
                ContentVersion: "1.0.0",
                DisasterDefinitions: new List<ReferenceDisasterDefinition>
                {
                    CreateDisaster("disaster-1", "Disaster 1"),
                    CreateDisaster("disaster-2", "Disaster 2"),
                    CreateDisaster("disaster-3", "Disaster 3")
                },
                CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>(),
                LocationDefinitions: new List<ReferenceLocationDefinition>()
            );

            // Act & Assert
            validator.Validate(snapshot);
        }

        [Fact]
        public void Validate_WhenDuplicateDisasterCodes_ThrowsReferenceDataCompilationException()
        {
            // Arrange
            var validator = new SnapshotValidator();
            var snapshot = new ReferenceDataSnapshot(
                SchemaVersion: 1,
                ContentVersion: "1.0.0",
                DisasterDefinitions: new List<ReferenceDisasterDefinition>
                {
                    CreateDisaster("duplicate-code", "Disaster 1"),
                    CreateDisaster("duplicate-code", "Disaster 2")
                },
                CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>(),
                LocationDefinitions: new List<ReferenceLocationDefinition>()
            );

            // Act & Assert
            var exception = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
            Assert.Contains("duplicate-code", exception.Message);
            Assert.Contains("Duplicate disaster codes", exception.Message);
        }

        [Fact]
        public void Validate_WhenDuplicateDisasterNames_ThrowsReferenceDataCompilationException()
        {
            // Arrange
            var validator = new SnapshotValidator();
            var snapshot = new ReferenceDataSnapshot(
                SchemaVersion: 1,
                ContentVersion: "1.0.0",
                DisasterDefinitions: new List<ReferenceDisasterDefinition>
                {
                    CreateDisaster("disaster-1", "Duplicate Name"),
                    CreateDisaster("disaster-2", "Duplicate Name")
                },
                CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>(),
                LocationDefinitions: new List<ReferenceLocationDefinition>()
            );

            // Act & Assert
            var exception = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
            Assert.Contains("Duplicate Name", exception.Message);
            Assert.Contains("Duplicate disaster names", exception.Message);
        }

        private static ReferenceDisasterDefinition CreateDisaster(string code, string name)
        {
            return new ReferenceDisasterDefinition(
                code: new CardCode(code),
                displayName: name,
                difficultyNumber: 1,
                location: new LocationCode("location"),
                rescueType: RescueType.Air,
                bonuses: new List<ReferenceDisasterBonus>
                {
                    new ReferenceDisasterBonus(new DisasterBonusKey("bonus"), 1, null)
                },
                rewards: new List<ReferenceDisasterReward>
                {
                    new ReferenceDisasterReward.PlayerChoice()
                }
            );
        }
    }
}