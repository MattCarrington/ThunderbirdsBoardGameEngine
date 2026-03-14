using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.UnitTests.Compilation
{
    public class SnapshotValidatorTests
    {
        public class EntityIntegrityTests
        {
            [Fact]
            public void Validate_WhenAllEntitiesUnique_DoesNotThrow()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>
                    {
                        CreateDisaster("disaster-1", "Disaster 1", "location-1"),
                        CreateDisaster("disaster-2", "Disaster 2", "location-1"),
                        CreateDisaster("disaster-3", "Disaster 3", "location-2")
                    },
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("location-1", "Location 1"),
                        CreateLocation("location-2", "Location 2")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
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
                        CreateDisaster("duplicate-code", "Disaster 1", "location-1"),
                        CreateDisaster("duplicate-code", "Disaster 2", "location-1")
                    },
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("location-1", "Location 1")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var exception = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("duplicate-code", exception.Message);
                Assert.Contains("disaster codes", exception.Message);
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
                        CreateDisaster("disaster-1", "Duplicate Name", "location-1"),
                        CreateDisaster("disaster-2", "Duplicate Name", "location-1")
                    },
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("location-1", "Location 1")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var exception = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("Duplicate Name", exception.Message);
                Assert.Contains("disaster names", exception.Message);
            }

            [Fact]
            public void Validate_WhenDuplicateLocationCodes_ThrowsReferenceDataCompilationException()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>(),
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("duplicate-location", "Location 1"),
                        CreateLocation("duplicate-location", "Location 2")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var exception = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("duplicate-location", exception.Message);
                Assert.Contains("location codes", exception.Message);
            }

            [Fact]
            public void Validate_WhenDuplicateLocationNames_ThrowsReferenceDataCompilationException()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>(),
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("location-1", "Duplicate Location"),
                        CreateLocation("location-2", "Duplicate Location")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var exception = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("Duplicate Location", exception.Message);
                Assert.Contains("location names", exception.Message);
            }

            [Fact]
            public void Validate_WhenMultipleDuplicateDisasterCodes_ReportsAllDuplicates()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>
                    {
                        CreateDisaster("duplicate-1", "Disaster 1", "location-1"),
                        CreateDisaster("duplicate-1", "Disaster 2", "location-1"),
                        CreateDisaster("duplicate-2", "Disaster 3", "location-1"),
                        CreateDisaster("duplicate-2", "Disaster 4", "location-1")
                    },
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("location-1", "Location 1")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var exception = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("duplicate-1", exception.Message);
                Assert.Contains("duplicate-2", exception.Message);
            }
        }

        private static ReferenceDisasterDefinition CreateDisaster(string code, string name, string locationCode)
        {
            return new ReferenceDisasterDefinition(
                code: new CardCode(code),
                displayName: name,
                difficultyNumber: 1,
                location: new LocationCode(locationCode),
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

        private static ReferenceLocationDefinition CreateLocation(string code, string displayName)
        {
            return new ReferenceLocationDefinition(
                code: new LocationCode(code),
                displayName: displayName
            );
        }
    }
}