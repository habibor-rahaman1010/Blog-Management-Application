using Blog.Management.Application.ApplicationDtos.CategorisDtos;
using Blog.Management.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Blog.Management.Application.ApplicationDtos.BlogPostDtos
{
    public record BlogPostCreateDto
    {
        [Required(ErrorMessage = "Id is required.")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 150 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [MinLength(20, ErrorMessage = "Content must be at least 20 characters long.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters.")]
        public string Author { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Cover image is required.")]
        public IFormFile? CoverImageFile { get; set; }

        public string CoverImageUrl { get; set; }

        [Required(ErrorMessage = "Category selection is required.")]
        public Guid CategoryId { get; set; }

        public IList<SelectListItem>? Categories { get; private set; }

        public void SetCategoryValues(IEnumerable<CategoryDto> categories)
        {
            Categories = RazorUtility.ConvertCategories(categories);
        }
    }
}
