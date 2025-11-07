namespace Blog.Management.Domain.Utilities
{
    public class PagedWithResult<TEntity>
    {
        public IEnumerable<TEntity> Items { get; init; } = new List<TEntity>();
        public int Total { get; init; }
        public int TotalDisplay { get; init; }
        public int PageIndex { get; init; }
        public int PageSize { get; init; }
    }
}
