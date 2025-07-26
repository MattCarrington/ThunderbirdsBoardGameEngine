using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Importer.Helpers
{
    
    public static class EnumDisplayMapper
    {
        public static BoardLocation ParseLocation(string input)
        {
            return input.Trim().ToLowerInvariant() switch
            {
                "europe" => BoardLocation.Europe,
                "asia" => BoardLocation.Asia,
                "north pacific" => BoardLocation.NorthPacific,
                "north america" => BoardLocation.NorthAmerica,
                "north atlantic" => BoardLocation.NorthAtlantic,
                "africa" => BoardLocation.Africa,
                "indian ocean" => BoardLocation.IndianOcean,
                "australia" => BoardLocation.Australia,
                "south pacific" => BoardLocation.SouthPacific,
                "south america" => BoardLocation.SouthAmerica,
                "south atlantic" => BoardLocation.SouthAtlantic,
                "geostationary orbit" => BoardLocation.GeoStationaryOrbit,
                "venus" => BoardLocation.Venus,
                "mercury" => BoardLocation.Mercury,
                "the sun" => BoardLocation.Sun,
                "the moon" => BoardLocation.Moon,
                "mars" => BoardLocation.Mars,
                "asteroid belt" => BoardLocation.AsteroidBelt,
                _ => throw new ArgumentException($"Unknown board location: '{input}'")
            };
        }

        public static RescueType ParseRescueType(string input)
        {
            return input.Trim().ToLowerInvariant() switch
            {
                "land" => RescueType.Land,
                "air" => RescueType.Air,
                "sea" => RescueType.Sea,
                "space" => RescueType.Space,
                _ => throw new ArgumentException($"Unknown rescue type: '{input}'")
            };
        }
    }
}

