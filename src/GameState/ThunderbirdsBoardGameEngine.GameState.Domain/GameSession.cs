using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain
{
    public class GameSession
    {
        public Guid GameId { get; set; }

        public Dictionary<ThunderbirdCode, LocationCode> VehicleDefinitions { get; set; }

        public GameSession(Guid gameId, Dictionary<ThunderbirdCode, LocationCode> vehicleDefinitions)
        {
            GameId = gameId;
            VehicleDefinitions = vehicleDefinitions;
        }

        public void MoveVehicleLocation(ThunderbirdCode thunderbirdCode, LocationCode newLocation)
        {
            if (VehicleDefinitions.ContainsKey(thunderbirdCode))
            {
                VehicleDefinitions[thunderbirdCode] = newLocation;
            }
            else
            {
                throw new Exception($"Thunderbird with code {thunderbirdCode} not found in the game session.");
            }
        }

        public LocationCode GetVehicleLocation(ThunderbirdCode thunderbirdCode)
        {
            if (VehicleDefinitions.TryGetValue(thunderbirdCode, out var location))
            {
                return location;
            }
            else
            {
                throw new Exception($"Thunderbird with code {thunderbirdCode} not found in the game session.");
            }
        }
    }
}
