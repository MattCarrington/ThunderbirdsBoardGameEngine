using System.Collections.ObjectModel;
using ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain
{
    public sealed class Game
    {
        private readonly Dictionary<ThunderbirdCode, LocationCode> _machinesState;
        private readonly Dictionary<CharacterCode, ThunderbirdCode> _charactersState;
        private readonly ReadOnlyDictionary<ThunderbirdCode, LocationCode> _machinesView;
        private readonly ReadOnlyDictionary<CharacterCode, ThunderbirdCode> _charactersView;

        private const int InitialStateVersion = 0;

        public Guid Id { get; private set; }

        public DateTimeOffset CreatedAtUtc { get; private set; }

        public string SetupVersion { get; private set; }

        public long StateVersion { get; private set; }

        public IReadOnlyDictionary<ThunderbirdCode, LocationCode> Machines => _machinesView;

        public IReadOnlyDictionary<CharacterCode, ThunderbirdCode> Characters => _charactersView;

        private Game(
            Guid id,
            DateTimeOffset createdAtUtc,
            string setupVersion,
            long stateVersion,
            IReadOnlyDictionary<ThunderbirdCode, LocationCode> machines,
            IReadOnlyDictionary<CharacterCode, ThunderbirdCode> characters)
        {
            Id = id;
            CreatedAtUtc = createdAtUtc;
            SetupVersion = setupVersion;
            StateVersion = stateVersion;

            _machinesState =
                new Dictionary<ThunderbirdCode, LocationCode>(machines);

            _charactersState =
                new Dictionary<CharacterCode, ThunderbirdCode>(characters);

            _machinesView =
                new ReadOnlyDictionary<ThunderbirdCode, LocationCode>(
                    _machinesState);

            _charactersView =
                new ReadOnlyDictionary<CharacterCode, ThunderbirdCode>(
                    _charactersState);
        }

        public static Game Create(
            Guid id,
            DateTimeOffset createdAtUtc,
            string setupVersion,
            IReadOnlyDictionary<ThunderbirdCode, LocationCode> machines,
            IReadOnlyDictionary<CharacterCode, ThunderbirdCode> characters)
        {
            return new Game(
                id,
                createdAtUtc,
                setupVersion,
                InitialStateVersion,
                machines,
                characters);
        }

        public static Game Restore(
            Guid id,
            DateTimeOffset createdAtUtc,
            string setupVersion,
            long stateVersion,
            IReadOnlyDictionary<ThunderbirdCode, LocationCode> machines,
            IReadOnlyDictionary<CharacterCode, ThunderbirdCode> characters)
        {
            return new Game(
                id,
                createdAtUtc,
                setupVersion,
                stateVersion,
                machines,
                characters);
        }

        public void MoveThunderbirdMachine(ThunderbirdCode machineCode, LocationCode newLocation)
        {
            if (!_machinesState.ContainsKey(machineCode))
            {
                throw ThunderbirdMachineNotFoundException.Create(machineCode);
            }

            if (!_charactersState.ContainsValue(machineCode))
            {
                throw ThunderbirdMachineCannotMoveException.NoCharactersAboard(machineCode);
            }

            _machinesState[machineCode] = newLocation;
        }

        public LocationCode GetThunderbirdMachineLocation(ThunderbirdCode machineCode)
        {
            if (!_machinesState.TryGetValue(machineCode, out var location))
            {
                throw ThunderbirdMachineNotFoundException.Create(machineCode);
            }

            return location;
        }

        public void TransferCharacters(IReadOnlyCollection<CharacterAssignment> assignments)
        {
            if (assignments.Count is < 1 or > 2)
            {
                throw new ArgumentException(
                    "One or two character assignments are required.",
                    nameof(assignments));
            }

            var duplicateCharacterExists = assignments
                .GroupBy(assignment => assignment.CharacterCode)
                .Any(group => group.Count() > 1);

            if (duplicateCharacterExists)
            {
                throw new ArgumentException(
                    "A character may only be assigned once per operation.",
                    nameof(assignments));
            }

            foreach (var assignment in assignments)
            {
                if (!_charactersState.TryGetValue(assignment.CharacterCode, out var currentMachine))
                {
                    throw CharacterNotFoundException.Create(assignment.CharacterCode);
                }

                if (!_machinesState.ContainsKey(assignment.TargetThunderbirdCode))
                {
                    throw ThunderbirdMachineNotFoundException.Create(assignment.TargetThunderbirdCode);
                }

                if (currentMachine == assignment.TargetThunderbirdCode)
                {
                    throw new ArgumentException(
                        "Every assignment must transfer its character to another machine.",
                        nameof(assignments));
                }
            }

            foreach (var assignment in assignments)
            {
                _charactersState[assignment.CharacterCode] = assignment.TargetThunderbirdCode;
            }
        }
    }
}
