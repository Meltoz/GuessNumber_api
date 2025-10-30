using Shared.Enums.Sorting;

namespace Shared
{
    public static class SortOptionFactory
    {
        public static SortOption<T> Create<T>(string? sortBy, string? direction) 
            where T: struct, Enum
        {
            var sort = Enum.TryParse<T>(sortBy, true, out var parsedSort) ? parsedSort : default;
            var dir = Enum.TryParse<SortDirection>(direction, true, out var parsedDirection) ? parsedDirection : SortDirection.Descending;

            return new SortOption<T> { SortBy = sort, Direction = dir };
        }
    }
}
