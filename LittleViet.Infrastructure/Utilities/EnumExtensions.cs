using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LittleViet.Infrastructure.Mvc;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var displayName = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault()
            .GetCustomAttribute<DisplayAttribute>()?
            .GetName();

        if (String.IsNullOrEmpty(displayName))
        {
            displayName = enumValue.ToString();
        }
        return displayName;
    }
}

