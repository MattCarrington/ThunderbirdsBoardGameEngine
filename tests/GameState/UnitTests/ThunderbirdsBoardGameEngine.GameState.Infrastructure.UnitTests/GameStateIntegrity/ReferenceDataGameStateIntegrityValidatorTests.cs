using NSubstitute;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.GameStateIntegrity;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Testing.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.UnitTests.GameStateIntegrity
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
                id: Guid.Empty,
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0.0",
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") },
                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") },
                    { new("character-3"), new("thunderbird-1") }
                },
                turnOrder: [new("character-1"), new("character-2")]);

            // Act & Assert
            Assert.Throws<InvalidGameStateException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidGameStateException_WhenCharacterCountGreaterThanReferenceData()
        {
            // Arrange
            var validator = CreateValidator();

            var game = Game.Create(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0.0",
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") }
                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") },
                    { new("character-3"), new("thunderbird-1") },
                    { new("character-4"), new("thunderbird-2") }
                },
                turnOrder: [new("character-1"), new("character-2")]);

            // Act & Assert
            Assert.Throws<InvalidGameStateException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidGameStateException_WhenCharacterCountLessThanReferenceData()
        {
            // Arrange
            var validator = CreateValidator();

            var game = Game.Create(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0.0",
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") }
                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") }
                },
                turnOrder: [new("character-1"), new("character-2")]);

            // Act & Assert
            Assert.Throws<InvalidGameStateException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidGameStateException_WhenThunderbirdCountGreaterThanReferenceData()
        {
            // Arrange
            var validator = CreateValidator();
            var game = Game.Create(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0.0",
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") },
                    { new("thunderbird-3"), new("location-1") }
                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") }
                },
                turnOrder: [new("character-1"), new("character-2")]);

            // Act & Assert
            Assert.Throws<InvalidGameStateException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidGameStateException_WhenCharacterDoesNotExistInReferenceData()
        {
            // Arrange
            var validator = CreateValidator();
            var unknownCharacter = new CharacterCode("unknown-character");

            var game = Game.Create(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0.0",
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") }
                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { unknownCharacter, new("thunderbird-2") }
                },
                turnOrder:
                [
                    new("character-1"),
                    unknownCharacter
                ]);

            // Act & Assert
            Assert.Throws<InvalidGameStateException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidGameStateException_WhenCharacterAssignmentDoesNotExistInReferenceData()
        {
            // Arrange
            var validator = CreateValidator();

            var game = Game.Create(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0.0",
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") }
                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-x") }, // character-2 is assigned to a non-existent thunderbird
                    { new("character-3"), new("thunderbird-1") }
                },
                turnOrder: [new("character-1"), new("character-2")]);

            // Act & Assert
            Assert.Throws<InvalidGameStateException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidGameStateException_WhenThunderbirdCountLessThanReferenceData()
        {
            // Arrange
            var validator = CreateValidator();
            var game = Game.Create(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0.0",
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") }
                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") },
                    { new("character-3"), new("thunderbird-1") }
                },
                turnOrder: [new("character-1"), new("character-2")]);

            // Act & Assert
            Assert.Throws<InvalidGameStateException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidGameStateException_WhenThunderbirdLocationDoesNotExistInReferenceData()
        {
            // Arrange
            var validator = CreateValidator();
            var game = Game.Create(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0.0",
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-x") } // location-x does not exist in reference data
                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") },
                    { new("character-3"), new("thunderbird-1") }
                },
                turnOrder: [new("character-1"), new("character-2")]);

            // Act & Assert
            Assert.Throws<InvalidGameStateException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldThrowInvalidGameStateException_WhenThunderbirdDoesNotExistInReferenceData()
        {
            // Arrange
            var validator = CreateValidator();
            var game = Game.Create(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0.0",
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-x"), new("location-2") } // thunderbird-x does not exist in reference data
                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") },
                    { new("character-3"), new("thunderbird-1") }
                },
                turnOrder: [new("character-1"), new("character-2")]);

            // Act & Assert
            Assert.Throws<InvalidGameStateException>(() => validator.Validate(game));
        }

        [Fact]
        public void Validate_ShouldNotThrow_WhenGameStateIsValid()
        {
            // Arrange
            var validator = CreateValidator();

            var game = Game.Create(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "1.0.0",
                machines: new Dictionary<ThunderbirdCode, LocationCode>
                {
                    { new("thunderbird-1"), new("location-1") },
                    { new("thunderbird-2"), new("location-2") }
                },
                characters: new Dictionary<CharacterCode, ThunderbirdCode>
                {
                    { new("character-1"), new("thunderbird-1") },
                    { new("character-2"), new("thunderbird-2") },
                    { new("character-3"), new("thunderbird-1") }
                },
                turnOrder: [new("character-1"), new("character-2")]);

            // Act & Assert
            var exception = Record.Exception(() => validator.Validate(game));
            Assert.Null(exception);
        }

        private static ReferenceDataGameStateIntegrityValidator CreateValidator()
        {
            var characterOne = ReferenceDefinitionFactory.Character(new CharacterCode("character-1"), "Character 1", null);
            var characterTwo = ReferenceDefinitionFactory.Character(new CharacterCode("character-2"), "Character 2", null);
            var characterThree = ReferenceDefinitionFactory.Character(new CharacterCode("character-3"), "Character 3", null);

            var thunderbirdOne = ReferenceDefinitionFactory.Thunderbird(
                code: new ThunderbirdCode("thunderbird-1"),
                displayName: "Thunderbird 1",
                domain: MovementDomain.Earth,
                topSpeed: 1,
                characterCapacity: 1);

            var thunderbirdTwo = ReferenceDefinitionFactory.Thunderbird(
                code: new ThunderbirdCode("thunderbird-2"),
                displayName: "Thunderbird 2",
                domain: MovementDomain.Space,
                topSpeed: 2,
                characterCapacity: 2);

            var locationOne = ReferenceDefinitionFactory.Location(new LocationCode("location-1"), "Location 1", MovementDomain.Earth);
            var locationTwo = ReferenceDefinitionFactory.Location(new LocationCode("location-2"), "Location 2", MovementDomain.Space);

            var locationCatalog = Substitute.For<ILocationDefinitionCatalog>();
            locationCatalog.Exists(locationOne.Code).Returns(true);
            locationCatalog.Exists(locationTwo.Code).Returns(true);

            var thunderbirdCatalog = Substitute.For<IThunderbirdDefinitionCatalog>();
            thunderbirdCatalog.TryGetByCode(thunderbirdOne.Code, out Arg.Any<ReferenceThunderbirdDefinition>()).Returns(x => { x[1] = thunderbirdOne; return true; });
            thunderbirdCatalog.TryGetByCode(thunderbirdTwo.Code, out Arg.Any<ReferenceThunderbirdDefinition>()).Returns(x => { x[1] = thunderbirdTwo; return true; });
            thunderbirdCatalog.GetAll().Returns(new[] { thunderbirdOne, thunderbirdTwo }.ToImmutableArray());

            var characterCatalog = Substitute.For<ICharacterDefinitionCatalog>();
            characterCatalog.GetAll().Returns(new[] { characterOne, characterTwo, characterThree }.ToImmutableArray());
            characterCatalog.GetByCode(characterOne.Code).Returns(characterOne);
            characterCatalog.GetByCode(characterTwo.Code).Returns(characterTwo);
            characterCatalog.GetByCode(characterThree.Code).Returns(characterThree);

            return new ReferenceDataGameStateIntegrityValidator(
                locationCatalog,
                thunderbirdCatalog,
                characterCatalog);
        }
    }
}
