using Shared.Enums.Sorting;

namespace Shared
{
    public class SortOption<T> where T: struct, Enum
    {
        public T SortBy { get; set; }
        public SortDirection Direction { get; set; }
    }
}
