using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Management.Web.Areas.Admin.Models.UserManagementModels
{
    public class AddUserClaimModel
    {
        public Guid UserId { get; set; }
        public SelectList Users { get; set; }
        public string ClaimName { get; set; } = string.Empty;
        public string ClaimValue { get; set; } = string.Empty;

        public AddUserClaimModel()
        {
            Users = new SelectList(Enumerable.Empty<SelectListItem>());
        }
    }
}
