using Autofac;
using Blog.Management.Application.ServiceInterfaces;
using Blog.Management.Application.Services;
using Blog.Management.Domain.RepositoryInterfaces;
using Blog.Management.Domain.UnitOfWorkInterface;
using Blog.Management.Domain.Utilities;
using Blog.Management.Infrastructure.DbContexts;
using Blog.Management.Infrastructure.Repositories;
using Blog.Management.Infrastructure.UnitOfWork;
using Blog.Management.Infrastructure.Utilities;
using Microsoft.Data.SqlClient;
using System.Data.Common;

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

            builder.RegisterType<BlogPostRepository>()
                .As<IBlogPostRepository>().InstancePerLifetimeScope();

            builder.RegisterType<BlogPostManagementService>()
                .As<IBlogPostManagementService>().InstancePerLifetimeScope();

            builder.RegisterType<FileService>().As<IFileService>().SingleInstance();


            builder.Register(c => new SqlConnection(_connectionString))
                .As<DbConnection>().InstancePerLifetimeScope();

            builder.RegisterType<SqlUtility>().As<ISqlUtility>().InstancePerLifetimeScope();

        }
    }
}