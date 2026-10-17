using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.TestData
{
    public static class TestLocationCodes
    {
        public static LocationCode Europe => new("europe");

        public static LocationCode Asia => new("asia");

        public static LocationCode Africa => new("africa");

        public static LocationCode Australia => new("australia");

        public static LocationCode NorthAmerica => new("north-america");

        public static LocationCode SouthAmerica => new("south-america");

        public static LocationCode NorthAtlantic => new("north-atlantic");

        public static LocationCode SouthAtlantic => new("south-atlantic");

        public static LocationCode SouthPacific => new("south-pacific");

        public static LocationCode GeoStationaryOrbit => new("geo-stationary-orbit");

        public static LocationCode TheMoon => new("the-moon");

        public static LocationCode TheSun => new("the-sun");
    }
}
