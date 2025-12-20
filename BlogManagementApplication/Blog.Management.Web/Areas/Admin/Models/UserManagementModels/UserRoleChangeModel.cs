using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Management.Web.Areas.Admin.Models.UserManagementModels
{
    public class UserRoleChangeModel
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public IList<SelectListItem> Users { get; set; }
        public IList<SelectListItem> Roles { get; set; }

        public UserRoleChangeModel()
        {
            Users = new List<SelectListItem>();
            Roles = new List<SelectListItem>();
        }
    }
}
