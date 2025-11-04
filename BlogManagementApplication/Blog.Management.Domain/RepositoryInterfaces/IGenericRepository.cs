using Blog.Management.Domain.Entities;
using Blog.Management.Domain.Utilities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Blog.Management.Domain.RepositoryInterfaces
{
    public interface IGenericRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey>
        where TEntity : class, IBaseEntity<TKey>
        where TKey : IComparable<TKey>
    {
        IEnumerable<TEntity> GetAll(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool isTrackingOff = false);


        IEnumerable<TEntity> GetAll(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool isTrackingOff = false);


        PagedWithResult<TEntity> GetPagedList(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            int pageIndex = 1,
            int pageSize = 10,
            bool isTrackingOff = false);


        Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool isTrackingOff = false,
            CancellationToken cancellationToken = default);


        Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool isTrackingOff = false,
            CancellationToken cancellationToken = default);


        Task<PagedWithResult<TEntity>> GetPagedListAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            int pageIndex = 1,
            int pageSize = 10,
            bool isTrackingOff = false,
            CancellationToken cancellationToken = default);


        Task<IEnumerable<TResult>> GetAllAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool isTrackingOff = false,
            CancellationToken cancellationToken = default)
            where TResult : class;


        IEnumerable<TEntity> GetAllDynamic(
            Expression<Func<TEntity, bool>>? filter = null,
            string? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool isTrackingOff = false);


        PagedWithResult<TEntity> GetPagedListDynamic(
            Expression<Func<TEntity, bool>>? filter = null,
            string? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            int pageIndex = 1,
            int pageSize = 10,
            bool isTrackingOff = false);


        Task<IEnumerable<TEntity>> GetAllDynamicAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            string? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool isTrackingOff = false,
            CancellationToken cancellationToken = default);


        Task<PagedWithResult<TEntity>> GetPagedListDynamicAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            string? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            int pageIndex = 1,
            int pageSize = 10,
            bool isTrackingOff = false,
            CancellationToken cancellationToken = default);


        Task<TResult> SingleOrDefaultAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool isTrackingOff = false,
            CancellationToken cancellationToken = default);
    }
}