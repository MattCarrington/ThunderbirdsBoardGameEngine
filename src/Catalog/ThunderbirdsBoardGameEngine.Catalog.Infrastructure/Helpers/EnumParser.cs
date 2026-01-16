namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Helpers
{
    internal static class EnumParser
    {
        public static TEnum ParseEnum<TEnum>(string value) where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Enum value cannot be null or empty");
            }

            if (Enum.TryParse<TEnum>(value, ignoreCase: true, out var result) &&
                Enum.IsDefined(typeof(TEnum), result))
            {
                return result;
            }

            throw new ArgumentException($"Invalid enum value: {value}");
        }
    }
}