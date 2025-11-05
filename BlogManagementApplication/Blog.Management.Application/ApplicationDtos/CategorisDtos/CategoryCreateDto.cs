
namespace Blog.Management.Application.ApplicationDtos.CategorisDtos
{
    public record CategoryCreateDto
    {
        public string Name { get; set; }
        public string CreatedBy { get; set; }
    }
}
