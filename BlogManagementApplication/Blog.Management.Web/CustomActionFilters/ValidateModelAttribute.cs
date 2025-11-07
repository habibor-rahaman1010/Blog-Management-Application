using Blog.Management.Web.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Management.Web.CustomActionFilters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Where(x => x.Value!.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage)
                    );

                var response = new ValidationErrorResponse()
                {
                    Success = false,
                    Message = "Validation failed!",
                    Errors = errors
                };
            }

            base.OnActionExecuting(context);
        }
    }
}