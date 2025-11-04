using Blog.Management.Domain.UnitOfWorkInterface;
using Microsoft.EntityFrameworkCore;

namespace Blog.Management.Infrastructure.UnitOfWork
{
    public class ApplicationUnitOfWork<TDbContext> : UnitOfWork<TDbContext>, IApplicationUnitOfWork where TDbContext : DbContext
    {
        public ApplicationUnitOfWork(TDbContext dbContext) : base(dbContext)
        {

        }
    }
}
