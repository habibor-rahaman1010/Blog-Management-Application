using Blog.Management.Application.ApplicationDtos.CategorisDtos;
using Blog.Management.Domain.Entities;
using Mapster;

namespace Blog.Management.Web.MapsterProfiles
{
    public class CategoryProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Category, CategoryCreateDto>();
        }
    }
}
