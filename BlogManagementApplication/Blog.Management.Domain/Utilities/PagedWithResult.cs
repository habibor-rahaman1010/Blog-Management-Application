namespace Blog.Management.Domain.Utilities
{
    public class PagedWithResult<TEntity>
    {
        public required IEnumerable<TEntity> Items { get; init; }
        public required int Total { get; init; }
        public required int TotalDisplay { get; init; }
        public required int PageIndex { get; init; }
        public required int PageSize { get; init; }
    }
}
