using ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Domain.UnitTests
{
    public class GameTests
    {
        [Fact]
        public void TransferCharacters_WhenSingleAssignmentIsValid_ShouldReassignCharacter()
        {
            // Arrange
            var game = CreateGame();

            var assignment = new CharacterAssignment(
                CharacterCode: new("character-1"),
                TargetThunderbirdCode: new("machine-2"));

            // Act
            game.TransferCharacters([assignment]);

            // Assert
            AssertCharacterAssignment(game, assignment.CharacterCode, assignment.TargetThunderbirdCode);
        }

        [Fact]
        public void TransferCharacters_WhenReciprocalSwapIsValid_ShouldReassignBothCharacters()
        {
            // Arrange
            var game = CreateGame();

            var assignments = new List<CharacterAssignment>
            {
                new(
                    CharacterCode: new("character-1"),
                    TargetThunderbirdCode: new("machine-2")),
                new(
                    CharacterCode: new("character-2"),
                    TargetThunderbirdCode: new("machine-1"))
            };

            // Act
            game.TransferCharacters(assignments);

            // Assert
            foreach (var assignment in assignments)
            {
                AssertCharacterAssignment(game, assignment.CharacterCode, assignment.TargetThunderbirdCode);
            }
        }

        [Fact]
        public void TransferCharacters_WhenCharacterDoesNotExist_ShouldThrowCharacterNotFoundException()
        {
            // Arrange
            var game = CreateGame();

            var assignment = new CharacterAssignment(
                CharacterCode: new("non-existent-character"),
                TargetThunderbirdCode: new("machine-2"));

            // Act & Assert
            Assert.Throws<CharacterNotFoundException>(() => game.TransferCharacters([assignment]));
        }

        [Fact]
        public void TransferCharacters_WhenDestinationMachineDoesNotExist_ShouldThrowThunderbirdMachineNotFoundException()
        {
            // Arrange
            var game = CreateGame();

            var assignment = new CharacterAssignment(
                CharacterCode: new("character-1"),
                TargetThunderbirdCode: new("non-existent-machine"));

            // Act & Assert
            Assert.Throws<ThunderbirdMachineNotFoundException>(() => game.TransferCharacters([assignment]));
        }

        [Fact]
        public void TransferCharacters_WhenCharacterAlreadyOccupiesDestination_ShouldThrow()
        {
            // Arrange
            var game = CreateGame();

            var assignment = new CharacterAssignment(
                CharacterCode: new("character-1"),
                TargetThunderbirdCode: new("machine-1"));

            // Act & Assert
            Assert.Throws<ArgumentException>(() => game.TransferCharacters([assignment]));
        }

        [Fact]
        public void TransferCharacters_WhenCharacterIsAssignedMoreThanOnce_ShouldThrow()
        {
            // Arrange
            var game = CreateGame();

            var assignments = new List<CharacterAssignment>
            {
                new(
                    CharacterCode: new("character-1"),
                    TargetThunderbirdCode: new("machine-2")),
                new(
                    CharacterCode: new("character-1"),
                    TargetThunderbirdCode: new("machine-1"))
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => game.TransferCharacters(assignments));
        }

        [Fact]
        public void TransferCharacters_WhenAnyAssignmentIsInvalid_ShouldNotApplyAnyAssignment()
        {
            var game = CreateGame();

            var assignments = new[]
            {
                new CharacterAssignment(
                    new("character-1"),
                    new("machine-2")),

                new CharacterAssignment(
                    new("missing-character"),
                    new("machine-1"))
            };

            Assert.Throws<CharacterNotFoundException>(() => game.TransferCharacters(assignments));

            AssertCharacterAssignment(game, new("character-1"), new("machine-1"));
        }

        [Fact]
        public void MoveThunderbirdMachine_WhenMachineHasAnOccupant_ShouldUpdateLocation()
        {
            // Arrange
            var game = CreateGame();

            var machineCode = new ThunderbirdCode("machine-1");

            var newLocation = new LocationCode("location-2");

            // Act
            game.MoveThunderbirdMachine(machineCode, newLocation);

            // Assert
            var actualLocation = game.Machines[machineCode];
            Assert.Equal(newLocation, actualLocation);
        }

        [Fact]
        public void MoveThunderbirdMachine_WhenMachineHasNoOccupants_ShouldThrow()
        {
            // Arrange
            var game = CreateGame();

            var machineCode = new ThunderbirdCode("machine-3");

            var newLocation = new LocationCode("location-2");

            // Act & Assert
            Assert.Throws<ThunderbirdMachineCannotMoveException>(() => game.MoveThunderbirdMachine(machineCode, newLocation));
        }

        [Fact]
        public void MoveThunderbirdMachine_WhenMachineDoesNotExist_ShouldThrow()
        {
            // Arrange
            var game = CreateGame();
            var machineCode = new ThunderbirdCode("missing-machine");
            var newLocation = new LocationCode("location-2");

            // Act & Assert
            Assert.Throws<ThunderbirdMachineNotFoundException>(() => game.MoveThunderbirdMachine(machineCode, newLocation));
        }

        private static Game CreateGame()
        {
            return Game.Create(
                id: Guid.NewGuid(),
                createdAtUtc: DateTimeOffset.UtcNow,
                setupVersion: "test",
                machines:
                    new Dictionary<ThunderbirdCode, LocationCode>
                    {
                        [new("machine-1")] = new("location-1"),
                        [new("machine-2")] = new("location-1"),
                        [new("machine-3")] = new("location-1")
                    },
                characters:
                    new Dictionary<CharacterCode, ThunderbirdCode>
                    {
                        [new("character-1")] = new("machine-1"),
                        [new("character-2")] = new("machine-2")
                    });
        }

        private static void AssertCharacterAssignment(Game game, CharacterCode characterCode, ThunderbirdCode expectedThunderbirdCode)
        {
            Assert.True(game.Characters.TryGetValue(characterCode, out var actualThunderbirdCode));
            Assert.Equal(expectedThunderbirdCode, actualThunderbirdCode);
        }
    }
}
