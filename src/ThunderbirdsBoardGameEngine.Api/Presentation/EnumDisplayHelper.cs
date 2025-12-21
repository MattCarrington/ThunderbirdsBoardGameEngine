using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ThunderbirdsBoardGameEngine.Api.Presentation
{
    public static class EnumDisplayHelper
    {
        public static string GetDisplayName<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            var member = typeof(TEnum).GetMember(value.ToString()).FirstOrDefault();
            var displayAttribute = member?.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? value.ToString();
        }
    }
}
