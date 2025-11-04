using Blog.Management.Domain.Entities;
using Blog.Management.Domain.RepositoryInterfaces;
using Blog.Management.Domain.Utilities;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Blog.Management.Infrastructure.Repositories
{
    public abstract class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
        where TEntity : class, IBaseEntity<TKey>
        where TKey : IComparable<TKey> 
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        protected GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public virtual void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Delete(TKey id)
        {
            var entityToDelete = _dbSet.Find(id);
            if (entityToDelete != null)
            {
                Delete(entityToDelete);
            }
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>>? filter = null)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter), "Delet filter not be null!");
            }

            var entitiesToDelete = _dbSet.Where(filter);
            if (entitiesToDelete.Any())
            {
                _dbSet.RemoveRange(entitiesToDelete);
            }
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            var entityToDelete = await _dbSet.FindAsync(id);
            if (entityToDelete != null)
            {
                await DeleteAsync(entityToDelete);
            }
        }

        public virtual Task DeleteAsync(TEntity entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);

            return Task.CompletedTask;
        }


        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter), "Delete filter must not be null!");
            }

            var entitiesToDelete = _dbSet.Where(filter);

            if (await entitiesToDelete.AnyAsync())
            {
                _dbSet.RemoveRange(entitiesToDelete);
            }
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();
            return query.ToList();
        }


        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();
            return await query.ToListAsync();
        }


        public virtual TEntity GetById(TKey id)
        {
            var item = _dbSet.Find(id);
            if (item == null)
            {
                return null;
            }
            return item;
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id)
        {
            var item = await _dbSet.FindAsync(id);
            if (item == null)
            {
                return null;
            }
            return item;
        }

        public virtual int GetCount(Expression<Func<TEntity, bool>>? filter = null)
        {
            int count = 0;
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();

            if (filter != null)
            {
                count = query.Count(filter);
            }
            else
            {
                count = query.Count();
            }
            return count;
        }

        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            int count = 0;
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();

            if (filter != null)
            {
                count = await query.CountAsync(filter);
            }
            else
            {
                count = await query.CountAsync();
            }

            return count;
        }

        public virtual void Update(TEntity entity)
        {
            if (!_dbSet.Local.Any(x => x == entity))
            {
                _dbSet.Attach(entity);
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            if (!_dbSet.Local.Any(x => x == entity))
            {
                _dbSet.Attach(entity);
                _dbContext.Entry(entity).State = EntityState.Modified;
            }

            return Task.CompletedTask;
        }


        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            return query.ToList();
        }


        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                var result = orderBy(query);
                if (isTrackingOff)
                {
                    return result.AsNoTracking().ToList();
                }
                else
                {
                    return result.ToList();
                }
            }
            else
            {
                if (isTrackingOff)
                {
                    return query.AsNoTracking().ToList();
                }
                else
                {
                    return query.ToList();
                }
            }
        }


        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool isTrackingOff = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                var result = orderBy(query);
                if (isTrackingOff)
                {
                    return await result.AsNoTracking().ToListAsync(cancellationToken);
                }
                else
                {
                    return await result.ToListAsync(cancellationToken);
                }
            }
            else
            {
                if (isTrackingOff)
                {
                    return await query.AsNoTracking().ToListAsync(cancellationToken);
                }
                else
                {
                    return await query.ToListAsync(cancellationToken);
                }
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool isTrackingOff = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (isTrackingOff)
            {
                return await query.AsNoTracking().ToListAsync(cancellationToken);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool isTrackingOff = false, CancellationToken cancellationToken = default) where TResult : class
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();

            if (isTrackingOff)
            {
                query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                return await orderBy(query).Select(selector!).ToListAsync(cancellationToken);
            }

            return await query.Select(selector!).ToListAsync(cancellationToken);
        }

        public virtual IEnumerable<TEntity> GetAllDynamic(Expression<Func<TEntity, bool>>? filter = null, string? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                var result = query.OrderBy(orderBy);
                if (isTrackingOff)
                {
                    return result.AsNoTracking().ToList();
                }
                else
                {
                    return result.ToList();
                }
            }
            else
            {
                if (isTrackingOff)
                {
                    return query.AsNoTracking().ToList();
                }
                else
                {
                    return query.ToList();
                }
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllDynamicAsync(Expression<Func<TEntity, bool>>? filter = null, string? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool isTrackingOff = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                var result = query.OrderBy(orderBy);
                if (isTrackingOff)
                {
                    return await result.AsNoTracking().ToListAsync(cancellationToken);
                }
                else
                {
                    return await query.ToListAsync(cancellationToken);
                }
            }
            else
            {
                if (isTrackingOff)
                {
                    return await query.AsNoTracking().ToListAsync(cancellationToken);
                }
                else
                {
                    return await query.ToListAsync(cancellationToken);
                }
            }
        }


        public virtual PagedWithResult<TEntity> GetPagedList(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();
            var total = query.Count();
            var totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                var result = orderBy(query).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                if (isTrackingOff)
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = result.AsNoTracking().ToList(),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
                else
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = result.ToList(),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
            }
            else
            {
                var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

                if (isTrackingOff)
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = result.AsNoTracking().ToList(),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
                else
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = result.ToList(),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
            }
        }

        public virtual async Task<PagedWithResult<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();
            var total = await query.CountAsync();
            var totalDisplay = await query.CountAsync();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = await query.CountAsync();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                var result = orderBy(query).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                if (isTrackingOff)
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = await result.AsNoTracking().ToListAsync(cancellationToken),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
                else
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = await result.ToListAsync(cancellationToken),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
            }
            else
            {
                var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

                if (isTrackingOff)
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = await result.AsNoTracking().ToListAsync(cancellationToken),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
                else
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = await result.ToListAsync(cancellationToken),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
            }
        }

        public virtual PagedWithResult<TEntity> GetPagedListDynamic(Expression<Func<TEntity, bool>>? filter = null, string? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();
            var total = query.Count();
            var totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                var result = query.OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                if (isTrackingOff)
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = result.AsNoTracking().ToList(),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
                else
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = result.ToList(),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
            }
            else
            {
                var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                if (isTrackingOff)
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = result.AsNoTracking().ToList(),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
                else
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = result.ToList(),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
            }
        }

        public virtual async Task<PagedWithResult<TEntity>> GetPagedListDynamicAsync(Expression<Func<TEntity, bool>>? filter = null, string? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();
            var total = await query.CountAsync();
            var totalDisplay = await query.CountAsync();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                var result = query.OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                if (isTrackingOff)
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = await result.AsNoTracking().ToListAsync(),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
                else
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = await result.ToListAsync(),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
            }
            else
            {
                var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                if (isTrackingOff)
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = await result.AsNoTracking().ToListAsync(),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
                else
                {
                    return new PagedWithResult<TEntity>
                    {
                        Items = await result.ToListAsync(),
                        Total = total,
                        TotalDisplay = totalDisplay,
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    };
                }
            }
        }

        public virtual async Task<TResult> SingleOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool isTrackingOff = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable<TEntity>();

            if (isTrackingOff)
            {
                query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                return await orderBy(query).Select(selector).FirstOrDefaultAsync(cancellationToken);
            }
            else
            {
                return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
            }
        }
    }
}