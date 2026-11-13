using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V2
{
    public sealed class StandardGameSetupFactory
    {
        public const string SetupVersion = "standard-v2";

        public Game Create(Guid id, DateTimeOffset createdAtUtc, IReadOnlyList<CharacterCode> turnOrder)
        {
            var machines = ThunderbirdMachineStartingLocations.GetStartingLocations();
            var characters = CharacterStartingPositions.GetStartingPositions();

            return Game.Create(id, createdAtUtc, SetupVersion, machines, characters, turnOrder);
        }
    }
}
