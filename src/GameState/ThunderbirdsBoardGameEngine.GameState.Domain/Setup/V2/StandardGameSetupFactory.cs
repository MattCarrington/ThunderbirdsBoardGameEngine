using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V2
{
    public sealed class StandardGameSetupFactory
    {
        public const string SetupVersion = "standard-v2";

        public Game Create(Guid id, DateTimeOffset createdAtUtc, IReadOnlyList<CharacterCode> turnOrder)
        {
            var machines = ThunderbirdMachineStartingLocations.GetStartingLocations();
            var characters = CharacterStartingPositions.GetStartingPositions();

            if (turnOrder.Contains(KnownCharacterCodes.John))
            {
                machines[KnownThunderbirdCodes.Thunderbird3] = KnownLocationCodes.GeoStationaryOrbit;
            }

            return Game.Create(id, createdAtUtc, SetupVersion, machines, characters, turnOrder);
        }
    }
}
