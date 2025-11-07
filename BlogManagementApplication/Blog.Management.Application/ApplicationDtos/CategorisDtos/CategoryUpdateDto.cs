
namespace Blog.Management.Application.ApplicationDtos.CategorisDtos
{
    public record CategoryUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
