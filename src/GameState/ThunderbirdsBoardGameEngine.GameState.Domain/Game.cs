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
        private readonly ReadOnlyCollection<CharacterCode> _turnOrder;

        private const int InitialStateVersion = 0;

        public Guid Id { get; private set; }

        public DateTimeOffset CreatedAtUtc { get; private set; }

        public string SetupVersion { get; private set; }

        public long StateVersion { get; private set; }

        public IReadOnlyDictionary<ThunderbirdCode, LocationCode> Machines => _machinesView;

        public IReadOnlyDictionary<CharacterCode, ThunderbirdCode> Characters => _charactersView;

        public IReadOnlyList<CharacterCode> TurnOrder => _turnOrder;

        public int ActiveTurnIndex { get; private set; }

        public CharacterCode ActiveCharacter => _turnOrder[ActiveTurnIndex];

        private Game(
            Guid id,
            DateTimeOffset createdAtUtc,
            string setupVersion,
            long stateVersion,
            IReadOnlyDictionary<ThunderbirdCode, LocationCode> machines,
            IReadOnlyDictionary<CharacterCode, ThunderbirdCode> characters,
            IReadOnlyList<CharacterCode> turnOrder,
            int activeTurnIndex)
        {
            ValidateTurnState(characters, turnOrder, activeTurnIndex);

            Id = id;
            CreatedAtUtc = createdAtUtc;
            SetupVersion = setupVersion;
            StateVersion = stateVersion;

            _machinesState = new Dictionary<ThunderbirdCode, LocationCode>(machines);
            _charactersState = new Dictionary<CharacterCode, ThunderbirdCode>(characters);
            _machinesView = new ReadOnlyDictionary<ThunderbirdCode, LocationCode>(_machinesState);
            _charactersView = new ReadOnlyDictionary<CharacterCode, ThunderbirdCode>(_charactersState);
            _turnOrder = Array.AsReadOnly(turnOrder.ToArray());
            ActiveTurnIndex = activeTurnIndex;
        }

        public static Game Create(
            Guid id,
            DateTimeOffset createdAtUtc,
            string setupVersion,
            IReadOnlyDictionary<ThunderbirdCode, LocationCode> machines,
            IReadOnlyDictionary<CharacterCode, ThunderbirdCode> characters, IReadOnlyList<CharacterCode> turnOrder)
        {
            return new Game(
                id,
                createdAtUtc,
                setupVersion,
                InitialStateVersion,
                machines,
                characters,
                turnOrder,
                0);
        }

        public static Game Restore(
            Guid id,
            DateTimeOffset createdAtUtc,
            string setupVersion,
            long stateVersion,
            IReadOnlyDictionary<ThunderbirdCode, LocationCode> machines,
            IReadOnlyDictionary<CharacterCode, ThunderbirdCode> characters,
            IReadOnlyList<CharacterCode> turnOrder,
            int activeTurnIndex)
        {
            return new Game(
                id,
                createdAtUtc,
                setupVersion,
                stateVersion,
                machines,
                characters,
                turnOrder,
                activeTurnIndex);
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

                if (!_machinesState.ContainsKey(assignment.DestinationThunderbirdCode))
                {
                    throw ThunderbirdMachineNotFoundException.Create(assignment.DestinationThunderbirdCode);
                }

                if (currentMachine == assignment.DestinationThunderbirdCode)
                {
                    throw new ArgumentException(
                        "Every assignment must transfer its character to another machine.",
                        nameof(assignments));
                }
            }

            foreach (var assignment in assignments)
            {
                _charactersState[assignment.CharacterCode] = assignment.DestinationThunderbirdCode;
            }
        }

        public ThunderbirdCode GetCharacterAssignment(CharacterCode characterCode)
        {
            if (!_charactersState.TryGetValue(characterCode, out var machine))
            {
                throw CharacterNotFoundException.Create(characterCode);
            }
            return machine;
        }

        public IReadOnlyCollection<CharacterCode> GetThunderbirdMachineOccupancy(ThunderbirdCode machineCode)
        {
            if (!_machinesState.ContainsKey(machineCode))
            {
                throw ThunderbirdMachineNotFoundException.Create(machineCode);
            }

            return _charactersState
                .Where(kvp => kvp.Value == machineCode)
                .Select(kvp => kvp.Key)
                .ToList()
                .AsReadOnly();
        }

        private static void ValidateTurnState(IReadOnlyDictionary<CharacterCode, ThunderbirdCode> characters, IReadOnlyList<CharacterCode> turnOrder, int activeTurnIndex)
        {
            ArgumentNullException.ThrowIfNull(turnOrder);

            if (turnOrder.Count is < 2 or > 4)
            {
                throw new ArgumentException(
                    "Turn order must contain between two and four characters.",
                    nameof(turnOrder));
            }

            if (turnOrder.Distinct().Count() != turnOrder.Count)
            {
                throw new ArgumentException(
                    "Turn order cannot contain duplicate characters.",
                    nameof(turnOrder));
            }

            if (turnOrder.Any(character => !characters.ContainsKey(character)))
            {
                throw new ArgumentException(
                    "Every turn-order character must exist in the game.",
                    nameof(turnOrder));
            }

            if (activeTurnIndex < 0
                || activeTurnIndex >= turnOrder.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(activeTurnIndex));
            }
        }
    }
}
