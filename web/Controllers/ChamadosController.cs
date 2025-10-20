using Microsoft.AspNetCore.Mvc;
using PIM.Web.Models;
using PIM.Web.Services;
using PIM.Web.Filters;

namespace PIM.Web.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de chamados
    /// </summary>
    [AuthorizeSession]
    public class ChamadosController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<ChamadosController> _logger;

        public ChamadosController(ApiService apiService, ILogger<ChamadosController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        /// <summary>
        /// Página de gerenciamento de chamados (Técnicos/Admin)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Gerenciar(int? status, int? categoria, int? prioridade, int? tecnico, string? busca)
        {
            try
            {
                var perfilAcessoId = HttpContext.Session.GetInt32("PerfilAcessoId");
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

                // Apenas técnicos e admins podem acessar
                if (perfilAcessoId == 3)
                {
                    TempData["Erro"] = "Acesso negado. Área restrita para técnicos.";
                    return RedirectToAction("Index");
                }

                // Buscar todos os chamados
                var chamados = await _apiService.GetAsync<List<ChamadoDto>>("/api/chamados");

                if (chamados == null)
                {
                    chamados = new List<ChamadoDto>();
                }

                // Aplicar filtros
                if (status.HasValue && status.Value > 0)
                {
                    chamados = chamados.Where(c => c.Status?.IdStatus == status.Value).ToList();
                }

                if (categoria.HasValue && categoria.Value > 0)
                {
                    chamados = chamados.Where(c => c.Categoria?.IdCategoria == categoria.Value).ToList();
                }

                if (prioridade.HasValue && prioridade.Value > 0)
                {
                    chamados = chamados.Where(c => c.Prioridade?.IdPrioridade == prioridade.Value).ToList();
                }

                if (tecnico.HasValue && tecnico.Value > 0)
                {
                    if (tecnico.Value == -1) // Não atribuídos
                    {
                        chamados = chamados.Where(c => c.UsuarioTecnico == null).ToList();
                    }
                    else
                    {
                        chamados = chamados.Where(c => c.UsuarioTecnico?.IdUsuario == tecnico.Value).ToList();
                    }
                }

                if (!string.IsNullOrWhiteSpace(busca))
                {
                    busca = busca.ToLower();
                    chamados = chamados.Where(c =>
                        c.Titulo.ToLower().Contains(busca) ||
                        c.Descricao.ToLower().Contains(busca) ||
                        c.IdChamado.ToString().Contains(busca)
                    ).ToList();
                }

                // Buscar listas para os filtros
                var categorias = await _apiService.GetAsync<List<CategoriaDto>>("/api/categorias") ?? new();
                var prioridades = await _apiService.GetAsync<List<PrioridadeDto>>("/api/prioridades") ?? new();
                var statusList = await _apiService.GetAsync<List<StatusDto>>("/api/status") ?? new();

                // Estatísticas rápidas
                ViewData["TotalChamados"] = chamados.Count;
                ViewData["ChamadosNaoAtribuidos"] = chamados.Count(c => c.UsuarioTecnico == null);
                ViewData["MeusChamados"] = chamados.Count(c => c.UsuarioTecnico?.IdUsuario == usuarioId);
                ViewData["ChamadosCriticos"] = chamados.Count(c =>
                    c.Prioridade?.Nivel?.ToLower() == "alta" ||
                    c.Prioridade?.Nivel?.ToLower() == "crítica");

                // Passar dados para a view
                ViewData["Categorias"] = categorias;
                ViewData["Prioridades"] = prioridades;
                ViewData["StatusList"] = statusList;
                ViewData["PerfilAcessoId"] = perfilAcessoId;
                ViewData["UsuarioId"] = usuarioId;

                // Filtros atuais
                ViewData["FiltroStatus"] = status;
                ViewData["FiltroCategoria"] = categoria;
                ViewData["FiltroPrioridade"] = prioridade;
                ViewData["FiltroTecnico"] = tecnico;
                ViewData["FiltroBusca"] = busca;

                return View(chamados.OrderByDescending(c => c.DataAbertura).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao carregar página de gerenciamento: {ex.Message}");
                TempData["Erro"] = "Erro ao carregar chamados";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Lista chamados do usuário logado ou todos (se técnico)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                var perfilAcessoId = HttpContext.Session.GetInt32("PerfilAcessoId");

                _logger.LogInformation($"[ChamadosController.Index] UsuarioId: {usuarioId}, PerfilAcessoId: {perfilAcessoId}");

                // Buscar chamados da API
                var chamados = await _apiService.GetAsync<List<ChamadoDto>>("/api/chamados");

                if (chamados == null)
                {
                    ViewBag.Erro = "Não foi possível carregar os chamados";
                    return View(new List<ChamadoDto>());
                }

                _logger.LogInformation($"[ChamadosController.Index] Total de chamados recebidos da API: {chamados.Count}");

                // Se for solicitante (perfil 3), mostrar apenas seus chamados
                if (perfilAcessoId == 3)
                {
                    _logger.LogInformation($"[ChamadosController.Index] Filtrando chamados para solicitante ID: {usuarioId}");

                    // Debug: mostrar IDs dos criadores
                    foreach (var c in chamados.Take(5))
                    {
                        _logger.LogInformation($"  Chamado #{c.IdChamado} - Criador ID: {c.UsuarioCriador?.IdUsuario}, Nome: {c.UsuarioCriador?.Nome}");
                    }

                    chamados = chamados.Where(c => c.UsuarioCriador != null && c.UsuarioCriador.IdUsuario == usuarioId).ToList();

                    _logger.LogInformation($"[ChamadosController.Index] Chamados após filtro: {chamados.Count}");
                }

                ViewData["PerfilAcessoId"] = perfilAcessoId;
                ViewData["UsuarioId"] = usuarioId;

                return View(chamados);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao listar chamados: {ex.Message}");
                ViewBag.Erro = "Erro ao carregar chamados";
                return View(new List<ChamadoDto>());
            }
        }

        /// <summary>
        /// Exibe o formulário para criar novo chamado
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Novo()
        {
            try
            {
                var viewModel = new NovoChamadoViewModel();

                // Buscar categorias e prioridades da API
                viewModel.Categorias = await _apiService.GetAsync<List<CategoriaDto>>("/api/categorias") ?? new();
                viewModel.Prioridades = await _apiService.GetAsync<List<PrioridadeDto>>("/api/prioridades") ?? new();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao carregar formulário de novo chamado: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Analisa o problema com IA antes de criar o chamado
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AnalisarProblema([FromBody] AnalisePreliminarRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { erro = "Dados inválidos", erros = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
                }

                // Chamar endpoint de IA da API
                var analise = await _apiService.PostAsync<AnalisePreliminarRequest, AnalisePreliminarResponse>("/api/ia/analisar-preliminar", request);

                if (analise == null)
                {
                    return StatusCode(500, new { erro = "Erro ao analisar problema com IA" });
                }

                return Ok(analise);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao analisar problema: {ex.Message}");
                return StatusCode(500, new { erro = "Erro ao processar análise", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo chamado
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Novo(NovoChamadoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Recarregar listas
                model.Categorias = await _apiService.GetAsync<List<CategoriaDto>>("/api/categorias") ?? new();
                model.Prioridades = await _apiService.GetAsync<List<PrioridadeDto>>("/api/prioridades") ?? new();
                return View(model);
            }

            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

                var novoChamado = new
                {
                    titulo = model.Titulo,
                    descricao = model.Descricao,
                    idUsuarioCriador = usuarioId,
                    idCategoria = model.IdCategoria,
                    idPrioridade = model.IdPrioridade,
                    idStatus = 1 // Status "Aberto"
                };

                var resultado = await _apiService.PostAsync<object, ChamadoDto>("/api/chamados", novoChamado);

                if (resultado != null)
                {
                    TempData["Sucesso"] = "Chamado criado com sucesso!";
                    return RedirectToAction("Detalhes", new { id = resultado.IdChamado });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Erro ao criar chamado. Tente novamente.");
                    model.Categorias = await _apiService.GetAsync<List<CategoriaDto>>("/api/categorias") ?? new();
                    model.Prioridades = await _apiService.GetAsync<List<PrioridadeDto>>("/api/prioridades") ?? new();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao criar chamado: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Erro ao criar chamado");
                model.Categorias = await _apiService.GetAsync<List<CategoriaDto>>("/api/categorias") ?? new();
                model.Prioridades = await _apiService.GetAsync<List<PrioridadeDto>>("/api/prioridades") ?? new();
                return View(model);
            }
        }

        /// <summary>
        /// Exibe os detalhes completos do chamado
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            try
            {
                var chamado = await _apiService.GetAsync<ChamadoDetalhadoDto>($"/api/chamados/{id}");

                if (chamado == null)
                {
                    TempData["Erro"] = "Chamado não encontrado";
                    return RedirectToAction("Index");
                }

                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                var perfilAcessoId = HttpContext.Session.GetInt32("PerfilAcessoId");

                // Verificar se o usuário tem permissão para ver este chamado
                if (perfilAcessoId == 3 && chamado.UsuarioCriador.IdUsuario != usuarioId)
                {
                    TempData["Erro"] = "Você não tem permissão para visualizar este chamado";
                    return RedirectToAction("Index");
                }

                ViewData["PerfilAcessoId"] = perfilAcessoId;
                ViewData["UsuarioId"] = usuarioId;

                return View(chamado);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao buscar detalhes do chamado {id}: {ex.Message}");
                TempData["Erro"] = "Erro ao carregar detalhes do chamado";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Técnico assume o chamado
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Assumir(int id)
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                var perfilAcessoId = HttpContext.Session.GetInt32("PerfilAcessoId");

                // Apenas técnicos podem assumir
                if (perfilAcessoId == 3)
                {
                    TempData["Erro"] = "Apenas técnicos podem assumir chamados";
                    return RedirectToAction("Detalhes", new { id });
                }

                var request = new
                {
                    idUsuarioTecnico = usuarioId
                };

                var sucesso = await _apiService.PutAsync<object, object>($"/api/chamados/{id}/atribuir", request);

                if (sucesso != null)
                {
                    TempData["Sucesso"] = "Chamado assumido com sucesso!";
                }
                else
                {
                    TempData["Erro"] = "Erro ao assumir chamado";
                }

                return RedirectToAction("Detalhes", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao assumir chamado {id}: {ex.Message}");
                TempData["Erro"] = "Erro ao assumir chamado";
                return RedirectToAction("Detalhes", new { id });
            }
        }

        /// <summary>
        /// Técnico altera o status do chamado
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarStatus(int id, int idStatus, string? observacao, string? solucaoFinal)
        {
            try
            {
                var perfilAcessoId = HttpContext.Session.GetInt32("PerfilAcessoId");

                // Apenas técnicos podem alterar status
                if (perfilAcessoId == 3)
                {
                    TempData["Erro"] = "Apenas técnicos podem alterar o status";
                    return RedirectToAction("Detalhes", new { id });
                }

                var request = new
                {
                    idStatus = idStatus,
                    observacao = observacao,
                    solucaoFinal = solucaoFinal
                };

                var sucesso = await _apiService.PutAsync<object, object>($"/api/chamados/{id}/status", request);

                if (sucesso != null)
                {
                    TempData["Sucesso"] = "Status atualizado com sucesso!";
                }
                else
                {
                    TempData["Erro"] = "Erro ao atualizar status";
                }

                return RedirectToAction("Detalhes", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao alterar status do chamado {id}: {ex.Message}");
                TempData["Erro"] = "Erro ao alterar status";
                return RedirectToAction("Detalhes", new { id });
            }
        }

        /// <summary>
        /// Usuário envia feedback para chamado resolvido
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarFeedback(int id, int nota, string? comentario)
        {
            try
            {
                var request = new
                {
                    nota = nota,
                    comentario = comentario
                };

                var sucesso = await _apiService.PostAsync<object, object>($"/api/chamados/{id}/feedback", request);

                if (sucesso != null)
                {
                    TempData["Sucesso"] = "Feedback enviado com sucesso!";
                }
                else
                {
                    TempData["Erro"] = "Erro ao enviar feedback";
                }

                return RedirectToAction("Detalhes", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao enviar feedback do chamado {id}: {ex.Message}");
                TempData["Erro"] = "Erro ao enviar feedback";
                return RedirectToAction("Detalhes", new { id });
            }
        }

        /// <summary>
        /// Obtém comentários de um chamado (para o chat)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObterComentarios(int idChamado)
        {
            try
            {
                var comentarios = await _apiService.GetAsync<List<ComentarioDto>>($"/api/chamados/{idChamado}/comentarios");
                return Ok(comentarios ?? new List<ComentarioDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter comentários: {ex.Message}");
                return StatusCode(500, new { erro = "Erro ao obter comentários" });
            }
        }

        /// <summary>
        /// Envia um comentário (fallback REST se SignalR falhar)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> EnviarComentario(int idChamado, [FromBody] object request)
        {
            try
            {
                var comentario = await _apiService.PostAsync<object, ComentarioDto>($"/api/chamados/{idChamado}/comentarios", request);
                return Ok(comentario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao enviar comentário: {ex.Message}");
                return StatusCode(500, new { erro = "Erro ao enviar comentário" });
            }
        }

        /// <summary>
        /// Marca comentários como lidos
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> MarcarComentariosLidos(int idChamado)
        {
            try
            {
                await _apiService.PutAsync<object, object>($"/api/chamados/{idChamado}/comentarios/marcar-lidos", new { });
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao marcar comentários como lidos: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
