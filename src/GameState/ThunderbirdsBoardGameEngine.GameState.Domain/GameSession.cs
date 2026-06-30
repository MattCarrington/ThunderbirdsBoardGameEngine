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
    }
}
