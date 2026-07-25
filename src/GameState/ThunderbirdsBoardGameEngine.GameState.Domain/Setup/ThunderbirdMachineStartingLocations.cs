using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain.Setup
{
    public sealed class ThunderbirdMachineStartingLocations
    {
        public static IDictionary<ThunderbirdCode, LocationCode> GetStartingLocations()
        {
            return new Dictionary<ThunderbirdCode, LocationCode>
            {
                { KnownThunderbirdCodes.Thunderbird1, KnownLocationCodes.SouthPacific },
                { KnownThunderbirdCodes.Thunderbird2, KnownLocationCodes.SouthPacific },
                { KnownThunderbirdCodes.Thunderbird3, KnownLocationCodes.SouthPacific },
                { KnownThunderbirdCodes.Thunderbird4, KnownLocationCodes.SouthPacific },
                { KnownThunderbirdCodes.Thunderbird5, KnownLocationCodes.GeoStationaryOrbit },
                { KnownThunderbirdCodes.Fab1, KnownLocationCodes.Europe }
            };
        }
    }
}
