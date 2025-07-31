using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.GameData.Domain.Enums
{
    public enum BoardLocation
    {
        Europe,
        Asia,

        [Display(Name = "North Pacific")]
        NorthPacific,

        [Display(Name = "North America")]
        NorthAmerica,

        [Display(Name = "North Atlantic")]
        NorthAtlantic,
        Africa,

        [Display(Name = "Indian Ocean")]
        IndianOcean,
        Australia,

        [Display(Name = "South Pacific")]
        SouthPacific,

        [Display(Name = "South America")]
        SouthAmerica,

        [Display(Name = "South Atlantic")]
        SouthAtlantic,

        [Display(Name = "Geo-Stationary Orbit")]
        GeoStationaryOrbit,
        Venus,
        Mercury,

        [Display(Name = "The Sun")]
        Sun,

        [Display(Name = "The Moon")]
        Moon,
        Mars,

        [Display(Name = "Asteroid Belt")]
        AsteroidBelt
    }
}
