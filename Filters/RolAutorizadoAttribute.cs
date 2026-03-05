using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AppEnvios.Filters
{
    public class RolAutorizadoAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public RolAutorizadoAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var rol = context.HttpContext.Session.GetString("Rol");

            if (rol == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (!_roles.Contains(rol))
            {
                context.Result = new RedirectToActionResult("Denegado", "Account", null);
            }
        }
    }
}