using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace LittleViet.Infrastructure.EntityFramework;

public static class SqlHelper
{
    //Wrapper around database CaseInsensitiveContains, currently using Pgsql Ilike function
    public static Expression<Func<TEntity, bool>> ContainsIgnoreCase<TEntity>(string propertyName,
        string pattern)
    {
        return ContainsIgnoreCase<TEntity>(new[] {propertyName}, pattern);
    }

    public static Expression<Func<TEntity, bool>> ContainsIgnoreCase<TEntity>(string[] propertyNames,
        string pattern, string separator = " ")
    {
        if (propertyNames == null || !propertyNames.Any()) throw new ArgumentNullException(nameof(propertyNames));

        var parameter = Expression.Parameter(typeof(TEntity), "x");

        var propertyInfos = propertyNames.Select(pn => typeof(TEntity).GetProperty(pn));

        var concatMethodInfo = typeof(string).GetMethod("Concat", new[] {typeof(string), typeof(string)});

        Expression leftComparison = propertyInfos.Aggregate((Expression) null, (accExpression, nextPropInfo) =>
            accExpression == null
                ? Expression.Property(parameter, nextPropInfo)
                : Expression.Add(
                    Expression.Add(
                        accExpression,
                        Expression.Constant(separator),
                        concatMethodInfo
                    ),
                    Expression.Property(parameter, nextPropInfo),
                    concatMethodInfo
                )
        );

        var likeMethodInfo = typeof(NpgsqlDbFunctionsExtensions).GetMethod("ILike",
            new[] {typeof(DbFunctions), typeof(string), typeof(string)});

        return Expression.Lambda<Func<TEntity, bool>>(
            Expression.Call(
                likeMethodInfo!,
                Expression.Constant(EF.Functions, typeof(DbFunctions)),
                leftComparison,
                Expression.Constant($"%{pattern}%", typeof(string))
            ),
            parameter);
        //Final lambda looks like x => EF.Functions.ILike((x.prop1 + " ") + prop2 ..., $"%{pattern}%))
    }
}