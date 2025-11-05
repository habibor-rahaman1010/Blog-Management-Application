using Mapster;
using MapsterMapper;
using System.Reflection;

namespace Blog.Management.Web.Extensions
{
    public static class MapsterConfiguration
    {
        public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }
    }
}
