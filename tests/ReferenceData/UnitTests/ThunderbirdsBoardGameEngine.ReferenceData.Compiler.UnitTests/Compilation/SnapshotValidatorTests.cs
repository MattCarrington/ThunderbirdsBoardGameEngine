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

        public class ReferentialIntegrityTests
        {
            [Fact]
            public void Validate_WhenDisasterReferencesValidLocation_DoesNotThrow()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>
                    {
                        CreateDisaster("disaster-1", "Disaster 1", "valid-location")
                    },
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("valid-location", "Valid Location")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert (should not throw)
                validator.Validate(snapshot);
            }

            [Fact]
            public void Validate_WhenDisasterReferencesInvalidLocation_Throws()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>
                    {
                        CreateDisaster("disaster-1", "Test Disaster", "invalid-location")
                    },
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("valid-location", "Valid Location")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var exception = Assert.Throws<ReferenceDataCompilationException>(() =>
                    validator.Validate(snapshot));
                Assert.Contains("invalid-location", exception.Message);
                Assert.Contains("Invalid location codes", exception.Message);
            }

            [Fact]
            public void Validate_WhenBonusHasNoLocation_DoesNotThrow()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var disaster = new ReferenceDisasterDefinition(
                    code: new CardCode("disaster-1"),
                    displayName: "Test Disaster",
                    difficultyNumber: 1,
                    location: new LocationCode("valid-location"),
                    rescueType: RescueType.Air,
                    bonuses: new List<ReferenceDisasterBonus>
                    {
                        // Bonus with NO location (null)
                        new ReferenceDisasterBonus(
                            key: new DisasterBonusKey("john"),
                            value: 2,
                            location: null)
                    },
                    rewards: new List<ReferenceDisasterReward>
                    {
                        new ReferenceDisasterReward.PlayerChoice()
                    }
                );

                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition> { disaster },
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("valid-location", "Valid Location")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert (should not throw - null locations are valid)
                validator.Validate(snapshot);
            }

            [Fact]
            public void Validate_WhenBonusReferencesValidLocation_DoesNotThrow()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var disaster = new ReferenceDisasterDefinition(
                    code: new CardCode("disaster-1"),
                    displayName: "Test Disaster",
                    difficultyNumber: 1,
                    location: new LocationCode("disaster-location"),
                    rescueType: RescueType.Air,
                    bonuses: new List<ReferenceDisasterBonus>
                    {
                        // Bonus WITH valid location
                        new ReferenceDisasterBonus(
                            key: new DisasterBonusKey("john"),
                            value: 2,
                            location: new LocationCode("bonus-location"))
                    },
                    rewards: new List<ReferenceDisasterReward>
                    {
                        new ReferenceDisasterReward.PlayerChoice()
                    }
                );

                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition> { disaster },
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("disaster-location", "Disaster Location"),
                        CreateLocation("bonus-location", "Bonus Location")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                validator.Validate(snapshot);
            }

            [Fact]
            public void Validate_WhenMultipleBonusesReferenceInvalidLocations_ReportsAll()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var disaster = new ReferenceDisasterDefinition(
                    code: new CardCode("disaster-1"),
                    displayName: "Test Disaster",
                    difficultyNumber: 1,
                    location: new LocationCode("valid-location"),
                    rescueType: RescueType.Air,
                    bonuses: new List<ReferenceDisasterBonus>
                    {
                        new ReferenceDisasterBonus(
                            key: new DisasterBonusKey("bonus1"),
                            value: 2,
                            location: new LocationCode("invalid-1")),
                        new ReferenceDisasterBonus(
                            key: new DisasterBonusKey("bonus2"),
                            value: 2,
                            location: new LocationCode("invalid-2"))
                    },
                    rewards: new List<ReferenceDisasterReward>
                    {
                        new ReferenceDisasterReward.PlayerChoice()
                    }
                );

                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition> { disaster },
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("valid-location", "Valid Location")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var exception = Assert.Throws<ReferenceDataCompilationException>(() =>
                    validator.Validate(snapshot));
                Assert.Contains("invalid-1", exception.Message);
                Assert.Contains("invalid-2", exception.Message);
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