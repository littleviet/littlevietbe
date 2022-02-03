using System.Linq.Dynamic.Core;

namespace LittleViet.Infrastructure.EntityFramework;

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string orderByQueryString)
    {
        return query.OrderBy(orderByQueryString);
    }
}