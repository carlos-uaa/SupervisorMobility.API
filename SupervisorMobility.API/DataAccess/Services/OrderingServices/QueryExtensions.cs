using System.Linq.Expressions;

namespace SupervisorMobility.API.DataAccess.Services.OrderingServices
{
    public static class QueryExtensions
    {
        public static IOrderedQueryable<TSource> OrderByDynamic<TSource, TKey>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            int? order)
        {
            switch (order)
            {
                case 1:
                    return source.OrderBy(keySelector);
                case 2:
                    return source.OrderByDescending(keySelector);
                default:
                    return (IOrderedQueryable<TSource>)source;
            }
        }
    }
}
