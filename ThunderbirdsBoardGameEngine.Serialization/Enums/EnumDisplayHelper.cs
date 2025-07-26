using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ThunderbirdsBoardGameEngine.Serialization.Enums
{
    public static class EnumDisplayHelper
    {
        public static TEnum ParseFromDisplayName<TEnum>(string input) where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input));

            var normalizedInput = Normalize(input);

            foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var displayAttr = field.GetCustomAttribute<DisplayAttribute>();
                var displayName = displayAttr?.Name ?? field.Name;

                if (Normalize(displayName) == normalizedInput)
                    return (TEnum)field.GetValue(null)!;
            }

            throw new ArgumentException($"Could not parse '{input}' to enum {typeof(TEnum).Name}");
        }

        public static string GetDisplayName<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            var member = typeof(TEnum).GetMember(value.ToString()).FirstOrDefault();
            var displayAttr = member?.GetCustomAttribute<DisplayAttribute>();

            return displayAttr?.Name ?? value.ToString();
        }

        private static string Normalize(string input) =>
            input.Trim().ToLowerInvariant().Replace(" ", "").Replace("-", "");
    }
}
