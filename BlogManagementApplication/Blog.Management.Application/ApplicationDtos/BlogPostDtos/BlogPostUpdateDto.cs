
namespace Blog.Management.Application.ApplicationDtos.BlogPostDtos
{
    public record BlogPostUpdateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public bool IsActive { get; set; }
        public string CoverImageUrl { get; set; }

        public Guid CategoryId { get; set; }
    }
}
