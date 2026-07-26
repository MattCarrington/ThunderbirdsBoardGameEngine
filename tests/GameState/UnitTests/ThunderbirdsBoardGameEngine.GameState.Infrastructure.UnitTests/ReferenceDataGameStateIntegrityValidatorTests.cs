using NSubstitute;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.UnitTests
{
    public class ReferenceDataGameStateIntegrityValidatorTests
    {
        [Fact]
        public void Validate_ShouldThrowArgumentNullException_WhenGameIsNull()
        {
            // Arrange
            var validator = CreateValidator();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => validator.Validate(null));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidOperationException_WhenGameIdIsEmpty()
        {
            // Arrange
            var validator = CreateValidator();

            var game = Game.Create(
                Guid.Empty,
                DateTimeOffset.UtcNow,
                "1.0.0",
                new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") }
                },
                new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") }
                });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidOperationException_WhenCharacterCountGreaterThanReferenceData()
        {
            // Arrange
            var validator = CreateValidator();

            var game = Game.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "1.0.0",
                new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") }
                },
                new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") },
                    { new("character-3"), new("thunderbird-1") }
                });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidOperationException_WhenCharacterCountLessThanReferenceData()
        {
            // Arrange
            var validator = CreateValidator();

            var game = Game.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "1.0.0",
                new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") }
                },
                new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") }
                });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidOperationException_WhenThunderbirdCountGreaterThanReferenceData()
        {
            // Arrange
            var validator = CreateValidator();
            var game = Game.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "1.0.0",
                new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") },
                    { new("thunderbird-3"), new("location-1") }
                },
                new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") }
                });
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidOperationException_WhenCharacterDoesNotExistInReferenceData()
        {
            // Arrange
            var validator = CreateValidator();

            var game = Game.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "1.0.0",
                new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") }
                },
                new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-3"), new("thunderbird-2") } // character-3 does not exist in reference data
                });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidOperationException_WhenCharacterAssignmentDoesNotExistInReferenceData()
        {
            // Arrange
            var validator = CreateValidator();

            var game = Game.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "1.0.0",
                new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") }
                },
                new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-x") } // character-2 is assigned to a non-existent thunderbird
                });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidOperationException_WhenThunderbirdCountLessThanReferenceData()
        {
            // Arrange
            var validator = CreateValidator();
            var game = Game.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "1.0.0",
                new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") }
                },
                new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") }
                });
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidOperationException_WhenThunderbirdLocationDoesNotExistInReferenceData()
        {
            // Arrange
            var validator = CreateValidator();
            var game = Game.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "1.0.0",
                new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-x") } // location-x does not exist in reference data
                },
                new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") }
                });
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidOperationException_WhenThunderbirdDoesNotExistInReferenceData()
        {
            // Arrange
            var validator = CreateValidator();
            var game = Game.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "1.0.0",
                new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-x"), new("location-2") } // thunderbird-x does not exist in reference data
                },
                new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") }
                });
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldNotThrow_WhenGameStateIsValid()
        {
            // Arrange
            var validator = CreateValidator();

            var game = Game.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "1.0.0",
                new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") }
                },
                new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") }
                });

            // Act & Assert
            var exception = Record.Exception(() => validator.Validate(game));
            Assert.Null(exception);
        }

        private static ReferenceDataGameStateIntegrityValidator CreateValidator()
        {
            var characterOne = new ReferenceCharacterDefinition(new CharacterCode("character-1"), "Character 1", null);
            var characterTwo = new ReferenceCharacterDefinition(new CharacterCode("character-2"), "Character 2", null);

            var thunderbirdOne = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-1"), "Thunderbird 1", MovementDomain.Earth, 1);
            var thunderbirdTwo = new ReferenceThunderbirdDefinition(new ThunderbirdCode("thunderbird-2"), "Thunderbird 2", MovementDomain.Space, 2);

            var locationOne = new ReferenceLocationDefinition(new LocationCode("location-1"), "Location 1", MovementDomain.Earth);
            var locationTwo = new ReferenceLocationDefinition(new LocationCode("location-2"), "Location 2", MovementDomain.Space);

            var locationCatalog = Substitute.For<ILocationDefinitionCatalog>();
            locationCatalog.Exists(locationOne.Code).Returns(true);
            locationCatalog.Exists(locationTwo.Code).Returns(true);

            var thunderbirdCatalog = Substitute.For<IThunderbirdDefinitionCatalog>();
            thunderbirdCatalog.TryGetByCode(thunderbirdOne.Code, out Arg.Any<ReferenceThunderbirdDefinition>()).Returns(x => { x[1] = thunderbirdOne; return true; });
            thunderbirdCatalog.TryGetByCode(thunderbirdTwo.Code, out Arg.Any<ReferenceThunderbirdDefinition>()).Returns(x => { x[1] = thunderbirdTwo; return true; });
            thunderbirdCatalog.GetAll().Returns(new[] { thunderbirdOne, thunderbirdTwo }.ToImmutableArray());

            var characterCatalog = Substitute.For<ICharacterDefinitionCatalog>();
            characterCatalog.GetAll().Returns(new[] { characterOne, characterTwo }.ToImmutableArray());
            characterCatalog.GetByCode(characterOne.Code).Returns(characterOne);
            characterCatalog.GetByCode(characterTwo.Code).Returns(characterTwo);

            return new ReferenceDataGameStateIntegrityValidator(
                locationCatalog,
                thunderbirdCatalog,
                characterCatalog);
        }
    }
}
