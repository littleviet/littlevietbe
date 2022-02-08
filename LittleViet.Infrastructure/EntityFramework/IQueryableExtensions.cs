using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace LittleViet.Infrastructure.Mvc;

public static class IQueryableExtensions
{
    public static IQueryable<TSource> ApplySort<TSource>(this IQueryable<TSource> query, string orderByQueryString)
    {
        return query.OrderBy(orderByQueryString); // TODO: make this work better, type strong?
    }

    public static IQueryable<TSource> Paginate<TSource>(this IQueryable<TSource> query, int pageNum, int pageSize)
    {
        return query.Skip((pageNum > 0 ? (pageNum - 1) : 0) * pageSize).Take(pageSize);
    }

    public static IQueryable<TSource> WhereIf<TSource>(
        this IQueryable<TSource> source, bool condition,
        Expression<Func<TSource, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }
}