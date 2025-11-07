using System.ComponentModel.DataAnnotations;

namespace Blog.Management.Application.ApplicationDtos.CategorisDtos
{
    public record CategoryCreateDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        public string Name { get; set; }
    }
}
