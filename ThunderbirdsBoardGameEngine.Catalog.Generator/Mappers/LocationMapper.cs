using ThunderbirdsBoardGameEngine.Catalog.Generator.Helpers;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Mappers
{
    public static class LocationMapper
    {
        public static string MapLocation(string location)
        {
            if (EnumDictionary.Location.TryGetValue(location.Trim(), out var output))
            {
                return output;
            }

            throw new ArgumentException($"Unknown location: '{location}'");
        }
    }
}
