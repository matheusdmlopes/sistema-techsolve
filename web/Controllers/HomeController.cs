using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PIM.Web.Models;
using PIM.Web.Filters;
using PIM.Web.Services;

namespace PIM.Web.Controllers;

[AuthorizeSession]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApiService _apiService;

    public HomeController(ILogger<HomeController> logger, ApiService apiService)
    {
        _logger = logger;
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["UsuarioNome"] = HttpContext.Session.GetString("UsuarioNome");
        var perfilAcessoId = HttpContext.Session.GetInt32("PerfilAcessoId");
        var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

        ViewData["PerfilAcessoId"] = perfilAcessoId;

        try
        {
            // Se for SOLICITANTE (perfil 3), dashboard personalizado
            if (perfilAcessoId == 3)
            {
                // Buscar TODOS os chamados e filtrar localmente
                var todosChamados = await _apiService.GetAsync<List<ChamadoDto>>("/api/chamados");

                if (todosChamados != null && usuarioId.HasValue)
                {
                    _logger.LogInformation($"Total de chamados no sistema: {todosChamados.Count}");
                    _logger.LogInformation($"Filtrando por usuário ID: {usuarioId.Value}");

                    // Filtrar apenas chamados do usuário logado
                    var meusChamados = todosChamados.Where(c => c.UsuarioCriador != null && c.UsuarioCriador.IdUsuario == usuarioId.Value).ToList();

                    _logger.LogInformation($"Meus chamados encontrados: {meusChamados.Count}");

                    // Calcular métricas locais (case-insensitive)
                    var metricasLocal = new DashboardMetricasDto
                    {
                        TotalChamados = meusChamados.Count,
                        ChamadosAbertos = meusChamados.Count(c => c.Status?.Nome?.Equals("Aberto", StringComparison.OrdinalIgnoreCase) == true),
                        ChamadosEmAndamento = meusChamados.Count(c => c.Status?.Nome?.Equals("Em Atendimento", StringComparison.OrdinalIgnoreCase) == true),
                        ChamadosResolvidos = meusChamados.Count(c => c.Status?.Nome?.Equals("Resolvido", StringComparison.OrdinalIgnoreCase) == true),
                        ChamadosFechados = meusChamados.Count(c => c.Status?.Nome?.Equals("Fechado", StringComparison.OrdinalIgnoreCase) == true),
                        TecnicosAtivos = 0, // Não relevante para solicitante
                        TempoMedioResolucaoHoras = null
                    };

                    _logger.LogInformation($"Métricas calculadas - Abertos: {metricasLocal.ChamadosAbertos}, Total: {metricasLocal.TotalChamados}");

                    ViewData["Metricas"] = metricasLocal;
                    ViewData["ChamadosRecentes"] = meusChamados.OrderByDescending(c => c.DataAbertura).Take(5).ToList();
                }
            }
            else
            {
                // TÉCNICO/ADMIN: Dashboard completo com métricas gerais
                var metricas = await _apiService.GetAsync<DashboardMetricasDto>("/api/dashboard/metricas");
                var chamadosRecentes = await _apiService.GetAsync<List<ChamadoDto>>("/api/chamados");

                if (metricas != null)
                {
                    ViewData["Metricas"] = metricas;
                }

                if (chamadosRecentes != null)
                {
                    ViewData["ChamadosRecentes"] = chamadosRecentes.OrderByDescending(c => c.DataAbertura).Take(5).ToList();
                }
            }

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao carregar dashboard: {ex.Message}");
            return View();
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
