using Blog.Management.Domain.RepositoryInterfaces;

namespace Blog.Management.Domain.UnitOfWorkInterface
{
    public interface IApplicationUnitOfWork : IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get; }
    }
}