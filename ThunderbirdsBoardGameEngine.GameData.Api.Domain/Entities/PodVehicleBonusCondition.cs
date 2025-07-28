using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities
{
    public sealed class PodVehicleBonusCondition : BonusCondition
    {
        public PodVehicle PodVehicle { get; set; }
    }
}
