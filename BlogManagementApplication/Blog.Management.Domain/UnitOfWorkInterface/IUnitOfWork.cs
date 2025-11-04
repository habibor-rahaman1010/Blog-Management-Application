namespace Blog.Management.Domain.UnitOfWorkInterface
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        public int SaveChanges();
        public Task<int> SaveChangesAsync();
        public Task BeginTransactionAsync();
        public Task CommitAsync();
        public Task RollbackAsync();
    }
}