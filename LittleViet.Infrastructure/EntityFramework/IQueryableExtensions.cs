using System.Linq.Dynamic.Core;

namespace LittleViet.Infrastructure.EntityFramework;

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string orderByQueryString)
    {
        return query.OrderBy(orderByQueryString); // TODO: make this work better, type strong?
    }
    
    public static IQueryable<TSource> Paginate<TSource>(this IQueryable<TSource> query, int pageNum, int pageSize)
    {
        return query.Skip((pageNum > 0 ? (pageNum - 1) : 0) * pageSize).Take(pageSize);
    }
}