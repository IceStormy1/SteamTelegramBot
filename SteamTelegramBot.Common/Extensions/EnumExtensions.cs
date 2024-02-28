using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SteamTelegramBot.Common.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Get value of attribute <see cref="DisplayAttribute.Name"/>
    /// </summary>
    public static string GetEnumDisplayName(this Enum property)
    {
        var enumDisplayName = property.GetType()
            .GetMember(property.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            ?.GetName();

        return enumDisplayName;
    }
}