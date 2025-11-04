using Blog.Management.Domain.UnitOfWorkInterface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Blog.Management.Infrastructure.UnitOfWork
{
    public class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private IDbContextTransaction? _dbTransaction;

        public UnitOfWork(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_dbTransaction != null)
            {
                return;
            }
            _dbTransaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await SaveChangesAsync();

                if (_dbTransaction != null)
                {
                    await _dbTransaction.CommitAsync();
                    await _dbTransaction.DisposeAsync();
                    _dbTransaction = null;
                }
            }
            catch (Exception ex)
            {
                await RollbackAsync();
                throw new InvalidOperationException("An unexpected error occurred during commit.", ex);
            }
        }

        public void Dispose()
        {
            _dbTransaction?.Dispose();
            _dbContext.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_dbTransaction != null)
            {
                await _dbTransaction.DisposeAsync();
            }

            await _dbContext.DisposeAsync();
        }

        public async Task RollbackAsync()
        {
            if (_dbTransaction != null)
            {
                await _dbTransaction.RollbackAsync();
                await _dbTransaction.DisposeAsync();
                _dbTransaction = null;
            }
        }
    }
}