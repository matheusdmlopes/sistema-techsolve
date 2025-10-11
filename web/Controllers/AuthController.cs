using Microsoft.AspNetCore.Mvc;
using PIM.Web.Models;
using PIM.Web.Services;

namespace PIM.Web.Controllers
{
    /// <summary>
    /// Controller responsável pela autenticação de usuários
    /// </summary>
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApiService apiService, ILogger<AuthController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        /// <summary>
        /// Exibe a página de login
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            // Se já estiver autenticado, redireciona para o dashboard
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        /// Processa o login do usuário
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Faz a requisição de login para a API
                var loginRequest = new
                {
                    email = model.Email,
                    senha = model.Senha
                };

                var response = await _apiService.PostAsync<object, LoginResponseDto>(
                    "/api/auth/login",
                    loginRequest
                );

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    // Armazena o token na sessão
                    HttpContext.Session.SetString("JwtToken", response.Token);
                    HttpContext.Session.SetString("UsuarioNome", response.Usuario.Nome);
                    HttpContext.Session.SetString("UsuarioEmail", response.Usuario.Email);
                    HttpContext.Session.SetInt32("UsuarioId", response.Usuario.Id);
                    HttpContext.Session.SetInt32("PerfilAcessoId", response.Usuario.PerfilAcessoId);

                    _logger.LogInformation($"Usuário {model.Email} autenticado com sucesso");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao realizar login: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Erro ao realizar login. Tente novamente.");
                return View(model);
            }
        }

        /// <summary>
        /// Realiza o logout do usuário
        /// </summary>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            _logger.LogInformation("Usuário realizou logout");
            return RedirectToAction("Login");
        }
    }
}
