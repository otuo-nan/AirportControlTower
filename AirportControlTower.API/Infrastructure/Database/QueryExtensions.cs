using System.Linq.Expressions;

namespace AirportControlTower.API.Infrastructure.Database
{
    public static class QueryExtensions
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> query,
              int pageNum, int pageSize)
        {
            pageNum = pageNum == 0 ? 1 : pageNum;

            if (pageSize == 0)
                throw new ArgumentOutOfRangeException
                    (nameof(pageSize), "page size cannot be zero");

            if (pageNum != 0)
                query = query.Skip((pageNum - 1) * pageSize);

            return query.Take(pageSize);
        }

        public static IQueryable<T> SortByDynamic<T>
          (this IQueryable<T> query, string orderByMember, string direction)
        {
            var queryElementTypeParam = Expression.Parameter(typeof(T));
            var memberAccess = Expression.PropertyOrField(queryElementTypeParam, orderByMember);
            var keySelector = Expression.Lambda(memberAccess, queryElementTypeParam);

            var orderBy = Expression.Call(
                typeof(Queryable),
                direction.Equals("ASC", StringComparison.CurrentCultureIgnoreCase) ? "OrderBy" : "OrderByDescending",
                [typeof(T), memberAccess.Type], query.Expression,
                Expression.Quote(keySelector));

            return query.Provider.CreateQuery<T>(orderBy);
        }
    }
}
