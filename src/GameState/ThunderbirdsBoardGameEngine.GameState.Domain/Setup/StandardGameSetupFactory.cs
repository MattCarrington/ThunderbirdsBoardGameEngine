namespace ThunderbirdsBoardGameEngine.GameState.Domain.Setup
{
    public sealed class StandardGameSetupFactory
    {
        public Game Create(Guid id, DateTimeOffset createdAtUtc, string setupVersion)
        {
            var machines = ThunderbirdMachineStartingLocations.GetStartingLocations();
            var characters = CharacterStartingPositions.GetStartingPositions();

            return Game.Create(id, createdAtUtc, setupVersion, machines, characters);
        }
    }
}
