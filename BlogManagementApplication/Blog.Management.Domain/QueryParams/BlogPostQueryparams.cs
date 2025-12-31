namespace Blog.Management.Application.ApplicationDtos.BlogPostDtos
{
    public class BlogPostQueryparams
    {
        public string? AuthorName { get; set; }
        public string? CategoryName { get; set; }
        public string? FromDate { get; set; }
        public string? Todate { get; set; }
        public string? SearchKeyWord { get; set; }
    }
}