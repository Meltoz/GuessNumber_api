using Shared;
using Shared.Enums.Sorting;
using System.Linq.Dynamic.Core;

namespace Infrastructure.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T, TSortEnum>(
            this IQueryable<T> query,
            SortOption<TSortEnum> options) 
            where TSortEnum : struct, Enum
        {
            if (options == null)
                return query;

            var propertyName = options.SortBy.ToString();

            var direction = options.Direction == SortDirection.Ascending ? "ascending" : "descending";
            query = query.OrderBy($"{propertyName} {direction}");

            return query;
        }
    }
}
