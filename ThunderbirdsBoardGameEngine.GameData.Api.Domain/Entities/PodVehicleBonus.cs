using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities
{
    public sealed class PodVehicleBonus : Bonus
    {
        public PodVehicle PodVehicle { get; set; }
    }
}
