using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Enums
{
    /// <summary>
    /// Represents the physical location associated with a the game board.
    /// </summary>
    /// <remarks>
    /// Values mirror the locations printed on the physical Thunderbirds
    /// game board. Display names are intended for UI presentation.
    /// </remarks>
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
