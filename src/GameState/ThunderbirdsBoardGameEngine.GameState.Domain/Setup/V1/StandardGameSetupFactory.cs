namespace ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1
{
    public sealed class StandardGameSetupFactory
    {
        public const string SetupVersion = "standard-v1";

        public Game Create(Guid id, DateTimeOffset createdAtUtc)
        {
            var machines = ThunderbirdMachineStartingLocations.GetStartingLocations();
            var characters = CharacterStartingPositions.GetStartingPositions();

            return Game.Create(id, createdAtUtc, SetupVersion, machines, characters);
        }
    }
}
