using System.Linq.Expressions;

namespace MealMind.Shared.Abstractions.QueriesAndCommands.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }


    // // IQueryable   – translated to SQL when using EF/Linq2Db/…
    // public static IQueryable<T> Where<T>(this IQueryable<T> source)
    //     where T : ISoftDelete
    //     => source.Where(e => !e.IsDeleted);
    //
    // // IEnumerable  – for in-memory collections
    // public static IEnumerable<T> Where<T>(this IEnumerable<T> source)
    //     where T : ISoftDelete
    //     => source.Where(e => !e.IsDeleted);

    // public static async Task<PaginatedList<TOut>> ToPagedListAsync<T, TOut>(
    //     this IQueryable<T> query,
    //     int page,
    //     int pageSize,
    //     Expression<Func<T, TOut>> mappingExpression,
    //     CancellationToken cancellationToken = default)
    // {
    //     var count = await query.CountAsync(cancellationToken);
    //
    //     var results = await query
    //         .Skip((page - 1) * pageSize)
    //         .Take(pageSize)
    //         .Select(mappingExpression)
    //         .ToListAsync(cancellationToken);
    //
    //     return new PaginatedList<TOut>(page, pageSize, count, results);
    // }
}