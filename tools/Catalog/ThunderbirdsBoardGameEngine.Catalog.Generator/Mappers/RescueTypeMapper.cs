using ThunderbirdsBoardGameEngine.Catalog.Generator.Helpers;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Mappers
{
    public static class RescueTypeMapper
    {
        public static string MapRescueType(string rescueType)
        {
            if (EnumDictionary.RescueType.TryGetValue(rescueType.Trim(), out var output))
            {
                return output;
            }

            throw new ArgumentException($"Unknown rescue type: '{rescueType}'");
        }
    }
}
