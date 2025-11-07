using Autofac;
using Blog.Management.Application.ServiceInterfaces;
using Blog.Management.Application.Services;
using Blog.Management.Domain.RepositoryInterfaces;
using Blog.Management.Domain.UnitOfWorkInterface;
using Blog.Management.Domain.Utilities;
using Blog.Management.Infrastructure.DbContexts;
using Blog.Management.Infrastructure.Repositories;
using Blog.Management.Infrastructure.UnitOfWork;

namespace Blog.Management.Web.AutofacModule
{
    public class WebModule : Module
    {
        private readonly string _connectionString;
        private readonly string _migrationAssembly;

        public WebModule(string connectionString, string migrationAssembly)
        {
            _connectionString = connectionString;
            _migrationAssembly = migrationAssembly;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationDbContext>().AsSelf()
               .WithParameter("connectionString", _connectionString)
               .WithParameter("migrationAssembly", _migrationAssembly)
               .InstancePerLifetimeScope();

            builder.RegisterType<BlogManagementDbContext>().AsSelf()
                .WithParameter("connectionString", _connectionString)
                .WithParameter("migrationAssembly", _migrationAssembly)
                .InstancePerLifetimeScope();

            builder.RegisterType<ApplicationUnitOfWork<ApplicationDbContext>>()
                .As<IApplicationUnitOfWork>().InstancePerLifetimeScope();

            builder.RegisterType<ApplicationUnitOfWork<BlogManagementDbContext>>()
                .As<IApplicationUnitOfWork>().InstancePerLifetimeScope();

            builder.RegisterType<ApplicationTime>()
                .As<IApplicationTime>().SingleInstance();

            builder.RegisterType<CategoryRepository>()
                .As<ICategoryRepository>().InstancePerLifetimeScope();

            builder.RegisterType<CategoryManagementService>()
                .As<ICategoryManagementService>().InstancePerLifetimeScope();

        }
    }
}