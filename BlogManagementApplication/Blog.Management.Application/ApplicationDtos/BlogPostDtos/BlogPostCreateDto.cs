using Blog.Management.Application.ApplicationDtos.CategorisDtos;
using Blog.Management.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Management.Application.ApplicationDtos.BlogPostDtos
{
    public record BlogPostCreateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public bool IsActive { get; set; }
        public IFormFile? CoverImageFile { get; set; }
        public string CoverImageUrl { get; set; }
        public Guid CategoryId { get; set; }

        public IList<SelectListItem>? Categories { get; private set; }
        public void SetCategoryValues(IEnumerable<CategoryDto> categories)
        {
            Categories = RazorUtility.ConvertCategories(categories);
        }
    }
}
