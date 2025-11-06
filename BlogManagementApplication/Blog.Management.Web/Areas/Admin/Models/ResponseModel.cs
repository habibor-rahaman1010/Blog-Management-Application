namespace Blog.Management.Web.Areas.Admin.Models
{
    public enum ResponseTypes
    {
        Success,
        Danger,
        Warning,
        Info,     
        Primary,
        Secondary
    }

    public class ResponseModel
    {
        public string? Message { get; set; }
        public ResponseTypes Type { get; set; }
    }
}
