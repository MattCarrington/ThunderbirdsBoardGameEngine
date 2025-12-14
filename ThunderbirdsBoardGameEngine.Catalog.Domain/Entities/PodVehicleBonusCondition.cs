using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    public sealed class PodVehicleBonusCondition : BonusCondition
    {
        public PodVehicle PodVehicle { get; }

        public PodVehicleBonusCondition(PodVehicle podVehicle, int value) : this(podVehicle, value, null)
        {
        }

        public PodVehicleBonusCondition(PodVehicle podVehicle, int value, BoardLocation? location) : base(value, location)
        {
            PodVehicle = podVehicle;
        }
    }
}
