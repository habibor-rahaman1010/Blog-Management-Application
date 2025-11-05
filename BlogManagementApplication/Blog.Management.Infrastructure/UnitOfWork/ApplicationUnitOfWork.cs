using Blog.Management.Domain.RepositoryInterfaces;
using Blog.Management.Domain.UnitOfWorkInterface;
using Microsoft.EntityFrameworkCore;

namespace Blog.Management.Infrastructure.UnitOfWork
{
    public class ApplicationUnitOfWork<TDbContext> : UnitOfWork<TDbContext>, IApplicationUnitOfWork where TDbContext : DbContext
    {
        public ICategoryRepository CategoryRepository { get; private set; }

        public ApplicationUnitOfWork(TDbContext dbContext,
            ICategoryRepository categoryRepository) : base(dbContext)
        {
            CategoryRepository = categoryRepository;
        }
    }
}
