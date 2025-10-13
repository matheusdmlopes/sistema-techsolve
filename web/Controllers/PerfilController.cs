using Microsoft.AspNetCore.Mvc;
using PIM.Web.Models;
using PIM.Web.Services;
using PIM.Web.Filters;

namespace PIM.Web.Controllers
{
    /// <summary>
    /// Controller para gerenciamento do perfil do usuário
    /// </summary>
    [AuthorizeSession]
    public class PerfilController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<PerfilController> _logger;

        public PerfilController(ApiService apiService, ILogger<PerfilController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        /// <summary>
        /// Exibe a página do perfil do usuário
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                var perfilAcessoId = HttpContext.Session.GetInt32("PerfilAcessoId");

                if (!usuarioId.HasValue)
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Buscar dados do usuário
                var usuario = await _apiService.GetAsync<UsuarioDetalhadoDto>($"/api/usuarios/{usuarioId.Value}");

                if (usuario == null)
                {
                    TempData["Erro"] = "Não foi possível carregar seus dados";
                    return RedirectToAction("Index", "Home");
                }

                // Buscar estatísticas do usuário
                try
                {
                    if (perfilAcessoId == 3) // Solicitante
                    {
                        var chamados = await _apiService.GetAsync<List<ChamadoDto>>("/api/chamados");
                        if (chamados != null)
                        {
                            var meusChamados = chamados.Where(c => c.UsuarioCriador?.IdUsuario == usuarioId.Value).ToList();

                            ViewData["TotalChamados"] = meusChamados.Count;
                            ViewData["ChamadosAbertos"] = meusChamados.Count(c => c.Status?.Nome == "Aberto");
                            ViewData["ChamadosResolvidos"] = meusChamados.Count(c => c.Status?.Nome == "Resolvido");
                        }
                    }
                    else // Técnico/Admin
                    {
                        var chamados = await _apiService.GetAsync<List<ChamadoDto>>("/api/chamados");
                        if (chamados != null)
                        {
                            var chamadosAtribuidos = chamados.Where(c => c.UsuarioTecnico?.IdUsuario == usuarioId.Value).ToList();

                            ViewData["TotalChamadosAtendidos"] = chamadosAtribuidos.Count;
                            ViewData["ChamadosEmAndamento"] = chamadosAtribuidos.Count(c =>
                                c.Status?.Nome == "Em Atendimento" || c.Status?.Nome == "Em Análise");
                            ViewData["ChamadosResolvidos"] = chamadosAtribuidos.Count(c =>
                                c.Status?.Nome == "Resolvido" || c.Status?.Nome == "Fechado");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Erro ao buscar estatísticas: {ex.Message}");
                }

                ViewData["PerfilAcessoId"] = perfilAcessoId;
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao carregar perfil: {ex.Message}");
                TempData["Erro"] = "Erro ao carregar perfil";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Atualiza os dados do perfil
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Atualizar(AtualizarPerfilViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

                if (!usuarioId.HasValue)
                {
                    return RedirectToAction("Login", "Auth");
                }

                var request = new
                {
                    nomeCompleto = model.NomeCompleto,
                    telefone = model.Telefone
                };

                var resultado = await _apiService.PutAsync<object, object>($"/api/usuarios/{usuarioId.Value}/perfil", request);

                if (resultado != null)
                {
                    // Atualizar nome na sessão
                    HttpContext.Session.SetString("UsuarioNome", model.NomeCompleto);
                    TempData["Sucesso"] = "Dados atualizados com sucesso!";
                }
                else
                {
                    TempData["Erro"] = "Erro ao atualizar dados";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar perfil: {ex.Message}");
                TempData["Erro"] = "Erro ao atualizar dados";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Altera a senha do usuário
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarSenha(AlterarSenhaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErroSenha"] = "Preencha todos os campos corretamente";
                return RedirectToAction("Index");
            }

            if (model.NovaSenha != model.ConfirmarSenha)
            {
                TempData["ErroSenha"] = "A nova senha e a confirmação não coincidem";
                return RedirectToAction("Index");
            }

            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

                if (!usuarioId.HasValue)
                {
                    return RedirectToAction("Login", "Auth");
                }

                var request = new
                {
                    senhaAtual = model.SenhaAtual,
                    novaSenha = model.NovaSenha
                };

                var resultado = await _apiService.PutAsync<object, object>($"/api/usuarios/{usuarioId.Value}/alterar-senha", request);

                if (resultado != null)
                {
                    TempData["Sucesso"] = "Senha alterada com sucesso!";
                }
                else
                {
                    TempData["ErroSenha"] = "Senha atual incorreta";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao alterar senha: {ex.Message}");
                TempData["ErroSenha"] = "Erro ao alterar senha";
                return RedirectToAction("Index");
            }
        }
    }
}
