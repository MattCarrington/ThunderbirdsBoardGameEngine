using System.Collections.ObjectModel;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain
{
    public sealed class Game
    {
        private readonly Dictionary<ThunderbirdCode, LocationCode> _machinesState;
        private readonly Dictionary<CharacterCode, ThunderbirdCode> _charactersState;
        private readonly ReadOnlyDictionary<ThunderbirdCode, LocationCode> _machinesView;
        private readonly ReadOnlyDictionary<CharacterCode, ThunderbirdCode> _charactersView;

        public Guid Id { get; private set; }

        public DateTimeOffset CreatedAtUtc { get; private set; }

        public string SetupVersion { get; private set; }

        public IReadOnlyDictionary<ThunderbirdCode, LocationCode> Machines => _machinesView;

        public IReadOnlyDictionary<CharacterCode, ThunderbirdCode> Characters => _charactersView;

        private Game(
            Guid id,
            DateTimeOffset createdAtUtc,
            string setupVersion,
            IReadOnlyDictionary<ThunderbirdCode, LocationCode> machines,
            IReadOnlyDictionary<CharacterCode, ThunderbirdCode> characters)
        {
            Id = id;
            CreatedAtUtc = createdAtUtc;
            SetupVersion = setupVersion;

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
                machines,
                characters);
        }

        public static Game Restore(
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
                machines,
                characters);
        }

        public void MoveThunderbirdMachine(ThunderbirdCode machineCode, LocationCode newLocation)
        {
            if (!_machinesState.ContainsKey(machineCode))
            {
                throw new ArgumentException(
                    $"Machine code '{machineCode}' does not exist in the game state.",
                    nameof(machineCode));
            }

            _machinesState[machineCode] = newLocation;
        }

        public LocationCode GetThunderbirdMachineLocation(ThunderbirdCode machineCode)
        {
            if (!_machinesState.TryGetValue(machineCode, out var location))
            {
                throw new ArgumentException(    // TODO: Decide on exception type across game state
                    $"Machine code '{machineCode}' does not exist in the game state.",
                    nameof(machineCode));
            }

            return location;
        }
    }
}
