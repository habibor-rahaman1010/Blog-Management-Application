using Blog.Management.Domain.Entities;
using System.Linq.Expressions;

namespace Blog.Management.Domain.RepositoryInterfaces
{
    public interface IBaseRepository<TEntity, TKey>
        where TEntity : class, IBaseEntity<TKey>
        where TKey : IComparable<TKey>
    {
        public void Add(TEntity entity);
        public Task AddAsync(TEntity entity);
        public void AddRange(IEnumerable<TEntity> entities);
        public Task AddRangeAsync(IEnumerable<TEntity> entities);
        public void Update(TEntity entity);
        public Task UpdateAsync(TEntity entity);

        public IEnumerable<TEntity> GetAll();
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public TEntity GetById(TKey id);
        public Task<TEntity> GetByIdAsync(TKey id);

        public int GetCount(Expression<Func<TEntity, bool>>? filter = null);
        public Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? filter = null);

        public void Delete(Expression<Func<TEntity, bool>>? filter = null);
        public void Delete(TEntity entityToDelete);
        public void Delete(TKey id);

        public Task DeleteAsync(Expression<Func<TEntity, bool>>? filter = null);
        public Task DeleteAsync(TEntity entityToDelete);
        public Task DeleteAsync(TKey id);
    }
}
