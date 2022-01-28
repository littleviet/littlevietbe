using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LittleViet.Infrastructure.Utilities;

public static class IQueryableExtensions
{
    public static IQueryable<TSource> Paginate<TSource>(this IQueryable<TSource> query, int pageNum, int pageSize)
    {
        return query.Skip((pageNum > 0 ? (pageNum - 1) : 0) * pageSize).Take(pageSize);
    }
}

