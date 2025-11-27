using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Enums
{
    public enum PodVehicle
    {
        Mole,
        Firefly,

        [Display(Name = "Mobile Crane")]
        MobileCrane,
        Excavator,
        Thunderizer,

        [Display(Name = "DOMO")]
        Domo,

        [Display(Name = "Transmitter Truck")]
        TransmitterTruck,

        [Display(Name = "Laser Cutter")]
        LaserCutter,

        [Display(Name = "Recovery Vehicles")]
        RecoveryVehicles,

        [Display(Name = "Elevator Cars")]
        ElevatorCars
    }
}
