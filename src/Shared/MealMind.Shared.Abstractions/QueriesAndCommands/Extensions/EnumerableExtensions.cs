using System.Linq.Expressions;

namespace MealMind.Shared.Abstractions.QueriesAndCommands.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.AsQueryable().Where(predicate).AsEnumerable() : query;
    }
}
