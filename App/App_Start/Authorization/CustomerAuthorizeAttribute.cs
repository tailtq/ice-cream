using System.Web.Mvc;

namespace App.App_Start.Authorization
{
    public class CustomerAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomerAuthorizeAttribute()
        {
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Customer/Login?returnUrl=" + filterContext.HttpContext.Request.Url.PathAndQuery);
        }
    }
}