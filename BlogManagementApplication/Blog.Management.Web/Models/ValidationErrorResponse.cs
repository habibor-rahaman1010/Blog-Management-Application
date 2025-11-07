namespace Blog.Management.Web.Models
{
    public class ValidationErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "Validation failed";
        public Dictionary<string, IEnumerable<string>> Errors { get; set; } = new Dictionary<string, IEnumerable<string>>();
    }
}
