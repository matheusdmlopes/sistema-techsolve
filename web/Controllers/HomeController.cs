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

        try
        {
            // Buscar métricas do dashboard da API
            var metricas = await _apiService.GetAsync<DashboardMetricasDto>("/api/dashboard/metricas");

            // Buscar chamados recentes
            var chamadosRecentes = await _apiService.GetAsync<List<ChamadoDto>>("/api/chamados");

            if (metricas != null)
            {
                ViewData["Metricas"] = metricas;
            }

            if (chamadosRecentes != null)
            {
                ViewData["ChamadosRecentes"] = chamadosRecentes.Take(5).ToList();
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
