using System.Web;
using System.Web.Mvc;

namespace butunislerburada.MVC.Attributes
{
    public class LoginSetting : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Request.Cookies["UserLogin"] == null)
            {
                filterContext.Result = new RedirectResult("/admin/login?returnUrl=" + HttpContext.Current.Request.Url.LocalPath);
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}