using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace LittleViet.Data.ServiceHelper;

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

public static class IQueryableExtensions
{
    public static IQueryable<TSource> Paginate<TSource>(this IQueryable<TSource> query, int pageNum, int pageSize)
    {
        return query.Skip((pageNum > 0 ? (pageNum - 1) : 0) * pageSize).Take(pageSize);
    }
}

