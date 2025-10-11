using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PIM.Web.Filters
{
    /// <summary>
    /// Atributo para verificar se o usuário está autenticado via sessão
    /// </summary>
    public class AuthorizeSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                // Redireciona para o login se não estiver autenticado
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
