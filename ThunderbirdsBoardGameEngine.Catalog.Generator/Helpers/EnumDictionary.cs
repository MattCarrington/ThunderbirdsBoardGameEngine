namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Helpers
{
    public static class EnumDictionary
    {
        private static readonly Dictionary<string, string> _rewardToken = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Teamwork"] = "teamwork",
            ["Intelligence"] = "intelligence",
            ["Logistics"] = "logistics",
            ["Determination"] = "determination",
            ["Technology"] = "technology"
        };

        private static readonly Dictionary<string, string> character = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Scott"] = "scott",
            ["Virgil"] = "virgil",
            ["Alan"] = "alan",
            ["Gordon"] = "gordon",
            ["John"] = "john",
            ["Lady Penelope"] = "ladyPenelope"
        };

        private static readonly Dictionary<string, string> _thunderbird = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Thunderbird 1"] = "thunderbird1",
            ["Thunderbird 2"] = "thunderbird2",
            ["Thunderbird 3"] = "thunderbird3",
            ["Thunderbird 4"] = "thunderbird4",
            ["Thunderbird 5"] = "thunderbird5",
            ["FAB 1"] = "fab1"
        };

        private static readonly Dictionary<string, string> _podVehicle = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Mole"] = "mole",
            ["DOMO"] = "domo",
            ["Transmitter Truck"] = "transmitterTruck",
            ["Elevator Cars"] = "elevatorCars",
            ["Firefly"] = "firefly",
            ["Thunderizer"] = "thunderizer",
            ["Recovery Vehicles"] = "recoveryVehicles",
            ["Mobile Crane"] = "mobileCrane",
            ["Excavator"] = "excavator",
            ["Laser Cutter"] = "laserCutter"
        };

        private static readonly Dictionary<string, string> _rescueType = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Air"] = "air",
            ["Sea"] = "sea",
            ["Space"] = "space",
            ["Land"] = "land"
        };

        private static readonly Dictionary<string, string> _location = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Europe"] = "europe",
            ["Asia"] = "asia",
            ["Africa"] = "africa",
            ["North Atlantic"] = "northAtlantic",
            ["South Atlantic"] = "southAtlantic",
            ["North Pacific"] = "northPacific",
            ["South Pacific"] = "southPacific",
            ["North America"] = "northAmerica",
            ["South America"] = "southAmerica",
            ["Indian Ocean"] = "indianOcean",
            ["Australia"] = "australia",
            ["The Sun"] = "theSun",
            ["Mercury"] = "mercury",
            ["Venus"] = "venus",
            ["Mars"] = "mars",
            ["The Moon"] = "theMoon",
            ["Asteroid Belt"] = "asteroidBelt",
            ["Geo-Stationary Orbit"] = "geoStationaryOrbit"
        };

        public static IReadOnlyDictionary<string, string> RewardToken => _rewardToken;

        public static IReadOnlyDictionary<string, string> Character => character;

        public static IReadOnlyDictionary<string, string> Thunderbird => _thunderbird;

        public static IReadOnlyDictionary<string, string> PodVehicle => _podVehicle;

        public static IReadOnlyDictionary<string, string> RescueType => _rescueType;

        public static IReadOnlyDictionary<string, string> Location => _location;
    }
}
