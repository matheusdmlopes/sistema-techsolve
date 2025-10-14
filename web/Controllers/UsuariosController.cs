using Microsoft.AspNetCore.Mvc;
using PIM.Web.Models;
using PIM.Web.Services;
using PIM.Web.Filters;

namespace PIM.Web.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de usuários (apenas Admin)
    /// </summary>
    [AuthorizeSession]
    public class UsuariosController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(ApiService apiService, ILogger<UsuariosController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os usuários (apenas Admin)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(int? perfil, int? setor, string? busca)
        {
            try
            {
                var perfilAcessoId = HttpContext.Session.GetInt32("PerfilAcessoId");

                // Apenas admins podem acessar
                if (perfilAcessoId != 1)
                {
                    TempData["Erro"] = "Acesso negado. Área restrita para administradores.";
                    return RedirectToAction("Index", "Home");
                }

                // Buscar usuários da API
                var usuarios = await _apiService.GetAsync<List<UsuarioListaDto>>("/api/usuarios");

                if (usuarios == null)
                {
                    usuarios = new List<UsuarioListaDto>();
                }

                // Aplicar filtros
                if (perfil.HasValue && perfil.Value > 0)
                {
                    usuarios = usuarios.Where(u => u.Perfil.Id == perfil.Value).ToList();
                }

                if (setor.HasValue && setor.Value > 0)
                {
                    usuarios = usuarios.Where(u => u.Setor.Id == setor.Value).ToList();
                }

                if (!string.IsNullOrWhiteSpace(busca))
                {
                    busca = busca.ToLower();
                    usuarios = usuarios.Where(u =>
                        u.NomeCompleto.ToLower().Contains(busca) ||
                        u.Email.ToLower().Contains(busca)
                    ).ToList();
                }

                // Buscar listas para filtros
                var perfis = await _apiService.GetAsync<List<PerfilAcessoDto>>("/api/perfis") ?? new();
                var setores = await _apiService.GetAsync<List<SetorDto>>("/api/setores") ?? new();

                ViewData["Perfis"] = perfis;
                ViewData["Setores"] = setores;
                ViewData["FiltroPerfil"] = perfil;
                ViewData["FiltroSetor"] = setor;
                ViewData["FiltroBusca"] = busca;

                return View(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao listar usuários: {ex.Message}");
                TempData["Erro"] = "Erro ao carregar usuários";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Exibe formulário para criar novo usuário
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Criar()
        {
            try
            {
                var perfilAcessoId = HttpContext.Session.GetInt32("PerfilAcessoId");

                if (perfilAcessoId != 1)
                {
                    TempData["Erro"] = "Acesso negado.";
                    return RedirectToAction("Index");
                }

                // Buscar listas
                var perfis = await _apiService.GetAsync<List<PerfilAcessoDto>>("/api/perfis") ?? new();
                var setores = await _apiService.GetAsync<List<SetorDto>>("/api/setores") ?? new();

                var model = new CriarUsuarioViewModel
                {
                    Perfis = perfis,
                    Setores = setores
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao carregar formulário de criação: {ex.Message}");
                TempData["Erro"] = "Erro ao carregar formulário";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(CriarUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Recarregar listas
                model.Perfis = await _apiService.GetAsync<List<PerfilAcessoDto>>("/api/perfis") ?? new();
                model.Setores = await _apiService.GetAsync<List<SetorDto>>("/api/setores") ?? new();
                return View(model);
            }

            try
            {
                var request = new
                {
                    nomeCompleto = model.NomeCompleto,
                    email = model.Email,
                    senha = model.Senha,
                    telefone = model.Telefone,
                    idSetor = model.IdSetor,
                    idPerfil = model.IdPerfil
                };

                var resultado = await _apiService.PostAsync<object, object>("/api/usuarios", request);

                if (resultado != null)
                {
                    TempData["Sucesso"] = "Usuário criado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Erro ao criar usuário. Verifique se o email já existe.");
                    model.Perfis = await _apiService.GetAsync<List<PerfilAcessoDto>>("/api/perfis") ?? new();
                    model.Setores = await _apiService.GetAsync<List<SetorDto>>("/api/setores") ?? new();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao criar usuário: {ex.Message}");
                ModelState.AddModelError("", "Erro ao criar usuário");
                model.Perfis = await _apiService.GetAsync<List<PerfilAcessoDto>>("/api/perfis") ?? new();
                model.Setores = await _apiService.GetAsync<List<SetorDto>>("/api/setores") ?? new();
                return View(model);
            }
        }

        /// <summary>
        /// Exibe formulário para editar usuário
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var perfilAcessoId = HttpContext.Session.GetInt32("PerfilAcessoId");

                if (perfilAcessoId != 1)
                {
                    TempData["Erro"] = "Acesso negado.";
                    return RedirectToAction("Index");
                }

                var usuario = await _apiService.GetAsync<UsuarioDetalhadoDto>($"/api/usuarios/{id}");

                if (usuario == null)
                {
                    TempData["Erro"] = "Usuário não encontrado";
                    return RedirectToAction("Index");
                }

                // Buscar listas
                var perfis = await _apiService.GetAsync<List<PerfilAcessoDto>>("/api/perfis") ?? new();
                var setores = await _apiService.GetAsync<List<SetorDto>>("/api/setores") ?? new();

                var model = new EditarUsuarioViewModel
                {
                    IdUsuario = usuario.IdUsuario,
                    NomeCompleto = usuario.NomeCompleto,
                    Email = usuario.Email,
                    Telefone = usuario.Telefone,
                    IdSetor = usuario.Setor.IdSetor,
                    IdPerfil = usuario.Perfil.IdPerfil,
                    Ativo = usuario.Ativo,
                    Perfis = perfis,
                    Setores = setores
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao carregar usuário para edição: {ex.Message}");
                TempData["Erro"] = "Erro ao carregar usuário";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(EditarUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Perfis = await _apiService.GetAsync<List<PerfilAcessoDto>>("/api/perfis") ?? new();
                model.Setores = await _apiService.GetAsync<List<SetorDto>>("/api/setores") ?? new();
                return View(model);
            }

            try
            {
                var request = new
                {
                    nomeCompleto = model.NomeCompleto,
                    telefone = model.Telefone,
                    idSetor = model.IdSetor,
                    idPerfil = model.IdPerfil
                };

                var resultado = await _apiService.PutAsync<object, object>($"/api/usuarios/{model.IdUsuario}", request);

                if (resultado != null)
                {
                    TempData["Sucesso"] = "Usuário atualizado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Erro ao atualizar usuário");
                    model.Perfis = await _apiService.GetAsync<List<PerfilAcessoDto>>("/api/perfis") ?? new();
                    model.Setores = await _apiService.GetAsync<List<SetorDto>>("/api/setores") ?? new();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao atualizar usuário: {ex.Message}");
                ModelState.AddModelError("", "Erro ao atualizar usuário");
                model.Perfis = await _apiService.GetAsync<List<PerfilAcessoDto>>("/api/perfis") ?? new();
                model.Setores = await _apiService.GetAsync<List<SetorDto>>("/api/setores") ?? new();
                return View(model);
            }
        }

        /// <summary>
        /// Ativa/Desativa um usuário
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAtivo(int id, bool ativo)
        {
            try
            {
                var perfilAcessoId = HttpContext.Session.GetInt32("PerfilAcessoId");

                if (perfilAcessoId != 1)
                {
                    TempData["Erro"] = "Acesso negado.";
                    return RedirectToAction("Index");
                }

                var endpoint = ativo ? $"/api/usuarios/{id}/ativar" : $"/api/usuarios/{id}/desativar";
                var resultado = await _apiService.PutAsync<object, object>(endpoint, new { });

                if (resultado != null)
                {
                    TempData["Sucesso"] = ativo ? "Usuário ativado com sucesso!" : "Usuário desativado com sucesso!";
                }
                else
                {
                    TempData["Erro"] = "Erro ao alterar status do usuário";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao ativar/desativar usuário: {ex.Message}");
                TempData["Erro"] = "Erro ao alterar status do usuário";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Reseta a senha de um usuário
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetarSenha(int id)
        {
            try
            {
                var perfilAcessoId = HttpContext.Session.GetInt32("PerfilAcessoId");

                if (perfilAcessoId != 1)
                {
                    TempData["Erro"] = "Acesso negado.";
                    return RedirectToAction("Index");
                }

                var request = new
                {
                    senha = "senha123", // Senha padrão
                    primeiroAcesso = true
                };

                var resultado = await _apiService.PutAsync<object, object>($"/api/usuarios/{id}", request);

                if (resultado != null)
                {
                    TempData["Sucesso"] = "Senha resetada para 'senha123'. Usuário deverá trocar no primeiro acesso.";
                }
                else
                {
                    TempData["Erro"] = "Erro ao resetar senha";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao resetar senha: {ex.Message}");
                TempData["Erro"] = "Erro ao resetar senha";
                return RedirectToAction("Index");
            }
        }
    }
}
