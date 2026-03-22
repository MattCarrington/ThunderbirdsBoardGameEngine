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
                var snapshot = CreateValidSnapshot();

                // Act & Assert
                validator.Validate(snapshot);
            }

            [Fact]
            public void Validate_WhenDuplicateDisasterCodes_Throws()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>
                    {
                        CreateDisaster("duplicate-code", "Disaster 1", "location-1", "character-1"),
                        CreateDisaster("duplicate-code", "Disaster 2", "location-1", "character-1")
                    },
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("location-1", "Location 1")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>
                    {
                        CreateCharacter("character-1", "Character 1")
                    },
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("duplicate-code", ex.Message);
                Assert.Contains("disaster codes", ex.Message);
            }

            [Fact]
            public void Validate_WhenDuplicateDisasterNames_Throws()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>
                    {
                        CreateDisaster("disaster-1", "Duplicate Name", "location-1", "character-1"),
                        CreateDisaster("disaster-2", "Duplicate Name", "location-1", "character-1")
                    },
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("location-1", "Location 1")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>
                    {
                        CreateCharacter("character-1", "Character 1")
                    },
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("Duplicate Name", ex.Message);
                Assert.Contains("disaster names", ex.Message);
            }

            [Fact]
            public void Validate_WhenDuplicateLocationCodes_Throws()
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
                var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("duplicate-location", ex.Message);
                Assert.Contains("location codes", ex.Message);
            }

            [Fact]
            public void Validate_WhenDuplicateLocationNames_Throws()
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
                var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("Duplicate Location", ex.Message);
                Assert.Contains("location names", ex.Message);
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
                        CreateDisaster("duplicate-1", "Disaster 1", "location-1", "character-1"),
                        CreateDisaster("duplicate-1", "Disaster 2", "location-1", "character-1"),
                        CreateDisaster("duplicate-2", "Disaster 3", "location-1", "character-1"),
                        CreateDisaster("duplicate-2", "Disaster 4", "location-1", "character-1")
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

            [Fact]
            public void Validate_WhenDuplicateCharacterCodes_Throws()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>(),
                    LocationDefinitions: new List<ReferenceLocationDefinition>(),
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>
                    {
                        CreateCharacter("duplicate-code", "Character 1"),
                        CreateCharacter("duplicate-code", "Character 2")
                    },
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("duplicate-code", ex.Message);
                Assert.Contains("character codes", ex.Message);
            }

            [Fact]
            public void Validate_WhenDuplicateCharacterNames_Throws()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>(),
                    LocationDefinitions: new List<ReferenceLocationDefinition>(),
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>
                    {
                        CreateCharacter("character-1", "Duplicate Name"),
                        CreateCharacter("character-2", "Duplicate Name")
                    },
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("Duplicate Name", ex.Message);
                Assert.Contains("character names", ex.Message);
            }

            [Fact]
            public void Validate_WhenDuplicateThunderbirdCodes_Throws()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>(),
                    LocationDefinitions: new List<ReferenceLocationDefinition>(),
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>
                    {
                        CreateThunderbird("duplicate-code", "Thunderbird 1"),
                        CreateThunderbird("duplicate-code", "Thunderbird 2")
                    },
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("duplicate-code", ex.Message);
                Assert.Contains("thunderbird codes", ex.Message);
            }

            [Fact]
            public void Validate_WhenDuplicateThunderbirdNames_Throws()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>(),
                    LocationDefinitions: new List<ReferenceLocationDefinition>(),
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>
                    {
                        CreateThunderbird("thunderbird-1", "Duplicate Name"),
                        CreateThunderbird("thunderbird-2", "Duplicate Name")
                    },
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("Duplicate Name", ex.Message);
                Assert.Contains("thunderbird names", ex.Message);
            }

            [Fact]
            public void Validate_WhenDuplicatePodVehicleCodes_Throws()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>(),
                    LocationDefinitions: new List<ReferenceLocationDefinition>(),
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>
                    {
                        CreatePodVehicle("duplicate-code", "Pod Vehicle 1"),
                        CreatePodVehicle("duplicate-code", "Pod Vehicle 2")
                    }
                );

                // Act & Assert
                var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("duplicate-code", ex.Message);
                Assert.Contains("pod vehicle codes", ex.Message);
            }

            [Fact]
            public void Validate_WhenDuplicatePodVehicleNames_Throws()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>(),
                    LocationDefinitions: new List<ReferenceLocationDefinition>(),
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>
                    {
                        CreatePodVehicle("pod-1", "Duplicate Name"),
                        CreatePodVehicle("pod-2", "Duplicate Name")
                    }
                );

                // Act & Assert
                var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("Duplicate Name", ex.Message);
                Assert.Contains("pod vehicle names", ex.Message);
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
                        // Use CreateDisaster with a valid bonus key
                        CreateDisaster("disaster-1", "Disaster 1", "valid-location", "character-1")
                    },
                    LocationDefinitions: new List<ReferenceLocationDefinition>
                    {
                        CreateLocation("valid-location", "Valid Location")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>
                    {
                        CreateCharacter("character-1", "Character 1")
                    },
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
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
                        CreateDisaster("disaster-1", "Disaster 1", "invalid-location", "bonus-key-1")
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
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>
                    {
                        CreateCharacter("john", "John")  // ← Add the referenced character!
                    },
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
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
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>
                    {
                        CreateCharacter("john", "John")  // ← Add the referenced character!
                    },
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

            [Fact]
            public void Validate_WhenBonusKeyReferencesValidCharacter_DoesNotThrow()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var disaster = new ReferenceDisasterDefinition(
                    code: new CardCode("disaster-1"),
                    displayName: "Test Disaster",
                    difficultyNumber: 1,
                    location: new LocationCode("location-1"),
                    rescueType: RescueType.Air,
                    bonuses: new List<ReferenceDisasterBonus>
                    {
                        new ReferenceDisasterBonus(
                            key: new DisasterBonusKey("scott"), // ← References character
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
                        CreateLocation("location-1", "Location 1")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>
                    {
                        CreateCharacter("scott", "Scott")
                    },
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                validator.Validate(snapshot);
            }

            [Fact]
            public void Validate_WhenBonusKeyReferencesValidThunderbird_DoesNotThrow()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var disaster = new ReferenceDisasterDefinition(
                    code: new CardCode("disaster-1"),
                    displayName: "Test Disaster",
                    difficultyNumber: 1,
                    location: new LocationCode("location-1"),
                    rescueType: RescueType.Air,
                    bonuses: new List<ReferenceDisasterBonus>
                    {
                        new ReferenceDisasterBonus(
                            key: new DisasterBonusKey("thunderbird-1"), // ← References thunderbird
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
                        CreateLocation("location-1", "Location 1")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>
                    {
                        CreateThunderbird("thunderbird-1", "Thunderbird 1")
                    },
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                validator.Validate(snapshot);
            }

            [Fact]
            public void Validate_WhenBonusKeyReferencesValidPodVehicle_DoesNotThrow()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var disaster = new ReferenceDisasterDefinition(
                    code: new CardCode("disaster-1"),
                    displayName: "Test Disaster",
                    difficultyNumber: 1,
                    location: new LocationCode("location-1"),
                    rescueType: RescueType.Air,
                    bonuses: new List<ReferenceDisasterBonus>
                    {
                        new ReferenceDisasterBonus(
                            key: new DisasterBonusKey("mole"), // ← References pod vehicle
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
                        CreateLocation("location-1", "Location 1")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>
                    {
                        CreatePodVehicle("mole", "The Mole")
                    }
                );

                // Act & Assert
                validator.Validate(snapshot);
            }

            [Fact]
            public void Validate_WhenBonusKeyReferencesInvalidEntity_Throws()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var disaster = new ReferenceDisasterDefinition(
                    code: new CardCode("disaster-1"),
                    displayName: "Test Disaster",
                    difficultyNumber: 1,
                    location: new LocationCode("location-1"),
                    rescueType: RescueType.Air,
                    bonuses: new List<ReferenceDisasterBonus>
                    {
                        new ReferenceDisasterBonus(
                            key: new DisasterBonusKey("invalid-entity"), // ← Not in any list!
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
                        CreateLocation("location-1", "Location 1")
                    },
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>
                    {
                        CreateCharacter("scott", "Scott")
                    },
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("invalid-entity", ex.Message);
                Assert.Contains("non-existent entities", ex.Message);
            }
        }

        public class CrossEntityUniquenessTests
        {
            [Fact]
            public void Validate_WhenEntityCodeDuplicatedAcrossTypes_Throws()
            {
                // Arrange
                var validator = new SnapshotValidator();
                var snapshot = new ReferenceDataSnapshot(
                    SchemaVersion: 1,
                    ContentVersion: "1.0.0",
                    DisasterDefinitions: new List<ReferenceDisasterDefinition>(),
                    LocationDefinitions: new List<ReferenceLocationDefinition>(),
                    CharacterDefinitions: new List<ReferenceCharacterDefinition>
                    {
                        CreateCharacter("duplicate-code", "Character")
                    },
                    ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>
                    {
                        CreateThunderbird("duplicate-code", "Thunderbird") // ← Same code!
                    },
                    PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>()
                );

                // Act & Assert
                var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
                Assert.Contains("duplicate-code", ex.Message);
                Assert.Contains("unique across all types", ex.Message);
            }
        }

        // ============================================
        // Helper Methods
        // ============================================

        private static ReferenceDataSnapshot CreateValidSnapshot()
        {
            return new ReferenceDataSnapshot(
                SchemaVersion: 1,
                ContentVersion: "1.0.0",
                DisasterDefinitions: new List<ReferenceDisasterDefinition>
                {
                    CreateDisaster("disaster-1", "Disaster 1", "location-1", "character-1")
                },
                LocationDefinitions: new List<ReferenceLocationDefinition>
                {
                    CreateLocation("location-1", "Location 1")
                },
                CharacterDefinitions: new List<ReferenceCharacterDefinition>
                {
                    CreateCharacter("character-1", "Character 1")
                },
                ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>
                {
                    CreateThunderbird("thunderbird-1", "Thunderbird 1")
                },
                PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>
                {
                    CreatePodVehicle("pod-1", "Pod 1")
                }
            );
        }

        private static ReferenceDisasterDefinition CreateDisaster(
            string code,
            string name,
            string locationCode,
            string bonusKeyCode)
        {
            return new ReferenceDisasterDefinition(
                code: new CardCode(code),
                displayName: name,
                difficultyNumber: 1,
                location: new LocationCode(locationCode),
                rescueType: RescueType.Air,
                bonuses: new List<ReferenceDisasterBonus>
                {
                    new ReferenceDisasterBonus(new DisasterBonusKey(bonusKeyCode), 1, null)
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

        private static ReferenceCharacterDefinition CreateCharacter(string code, string displayName)
        {
            return new ReferenceCharacterDefinition(
                code: new CharacterCode(code),
                displayName: displayName,
                rescueBonus: null
            );
        }

        private static ReferenceThunderbirdDefinition CreateThunderbird(string code, string displayName)
        {
            return new ReferenceThunderbirdDefinition(
                code: new ThunderbirdCode(code),
                displayName: displayName
            );
        }

        private static ReferencePodVehicleDefinition CreatePodVehicle(string code, string displayName)
        {
            return new ReferencePodVehicleDefinition(
                code: new PodVehicleCode(code),
                displayName: displayName
            );
        }
    }
}