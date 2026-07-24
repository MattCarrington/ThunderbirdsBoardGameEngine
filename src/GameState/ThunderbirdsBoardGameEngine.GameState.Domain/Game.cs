using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain
{
    public sealed class Game
    {
        private readonly IDictionary<ThunderbirdCode, LocationCode> _machinesState;
        private readonly IDictionary<CharacterCode, ThunderbirdCode> _charactersState;

        public Guid Id { get; private set; }

        public DateTimeOffset CreatedAtUtc { get; private set; }

        public string SetupVersion { get; private set; }

        public IDictionary<ThunderbirdCode, LocationCode> Machines => _machinesState;

        public IDictionary<CharacterCode, ThunderbirdCode> Characters => _charactersState;

        private Game(
        Guid id,
        DateTimeOffset createdAtUtc,
        string setupVersion,
        IDictionary<ThunderbirdCode, LocationCode> machines,
        IDictionary<CharacterCode, ThunderbirdCode> characters)
        {
            Id = id;
            CreatedAtUtc = createdAtUtc;
            SetupVersion = setupVersion;
            _machinesState = machines;
            _charactersState = characters;
        }

        public static Game Create(
            Guid id,
            DateTimeOffset createdAtUtc,
            string setupVersion,
            IDictionary<ThunderbirdCode, LocationCode> machines,
            IDictionary<CharacterCode, ThunderbirdCode> characters)
        {
            var characterStates = characters.ToArray();

            return new Game(
                id,
                createdAtUtc,
                setupVersion,
                machines,
                characters);
        }
    }
}
