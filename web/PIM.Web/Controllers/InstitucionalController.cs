using Microsoft.AspNetCore.Mvc;

namespace PIM.Web.Controllers
{
    /// <summary>
    /// Controller para páginas institucionais (Privacidade, Manual, etc.)
    /// </summary>
    public class InstitucionalController : Controller
    {
        /// <summary>
        /// Política de Privacidade e LGPD
        /// </summary>
        [HttpGet]
        public IActionResult Privacidade()
        {
            return View();
        }

        /// <summary>
        /// Manual do Usuário
        /// </summary>
        [HttpGet]
        public IActionResult Manual()
        {
            return View();
        }
    }
}
