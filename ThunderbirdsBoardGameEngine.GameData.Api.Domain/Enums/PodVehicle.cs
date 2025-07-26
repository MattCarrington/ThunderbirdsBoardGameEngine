using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums
{
    public enum PodVehicle
    {
        Mole,
        Firefly,

        [Display(Name = "Mobile Crane")]
        MobileCrane,
        Excavator,
        Thunderizer,
        DOMO,

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
