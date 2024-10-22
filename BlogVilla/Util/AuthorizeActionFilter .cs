using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BlogVilla.Util
{
    public class AuthorizeActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if the user is authenticated
            if (!Auth.IsLoggedIn(context.HttpContext)) // Use your custom authentication check
            {
                Message.SetMessage(context.HttpContext, "Unautherized request.Please Login first!!", "error");

                // Redirect to the login page if the user is not authenticated
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing
        }
    }
}
