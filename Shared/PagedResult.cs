namespace Shared
{
    public record PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; }

        public int TotalCount { get; set; }
    }
}
