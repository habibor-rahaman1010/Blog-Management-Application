namespace Blog.Management.Web.Areas.Admin.Models.UserManagementModels
{
    public class RoleUpdateModel
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
