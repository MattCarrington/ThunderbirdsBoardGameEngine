using ThunderbirdsBoardGameEngine.GameData.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Domain.Entities
{
    public sealed class PodVehicleBonusCondition : BonusCondition
    {
        public PodVehicle PodVehicle { get; set; }
    }
}
