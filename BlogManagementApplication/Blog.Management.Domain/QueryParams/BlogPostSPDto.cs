
namespace Blog.Management.Domain.QueryParams
{
    public record BlogPostSPDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public bool IsActive { get; set; }
        public string CoverImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
