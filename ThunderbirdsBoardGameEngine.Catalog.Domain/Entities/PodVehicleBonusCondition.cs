using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    public sealed class PodVehicleBonusCondition : BonusCondition
    {
        public PodVehicle PodVehicle { get; set; }
    }
}
