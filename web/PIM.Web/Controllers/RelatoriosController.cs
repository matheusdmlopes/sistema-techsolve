using Microsoft.AspNetCore.Mvc;
using PIM.Web.Models;
using PIM.Web.Services;
using PIM.Web.Filters;

namespace PIM.Web.Controllers
{
    /// <summary>
    /// Controller para relatórios e dashboards gerenciais
    /// </summary>
    [AuthorizeSession]
    public class RelatoriosController : Controller
    {
        private static readonly string[] OrdemPrioridades = ["Crítica", "Alta", "Média", "Baixa"];

        private readonly ApiService _apiService;
        private readonly ILogger<RelatoriosController> _logger;

        public RelatoriosController(ApiService apiService, ILogger<RelatoriosController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        /// <summary>
        /// Página principal de relatórios
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? dataInicio, DateTime? dataFim)
        {
            try
            {
                var perfilAcessoId = HttpContext.Session.GetInt32("PerfilAcessoId");

                // Apenas técnicos e admins podem acessar
                if (perfilAcessoId == 3)
                {
                    TempData["Erro"] = "Acesso negado. Área restrita para técnicos.";
                    return RedirectToAction("Index", "Home");
                }

                // Definir período padrão (último mês)
                if (!dataInicio.HasValue)
                {
                    dataInicio = DateTime.Now.AddMonths(-1);
                }

                if (!dataFim.HasValue)
                {
                    dataFim = DateTime.Now;
                }

                // Buscar dados da API
                var chamados = await _apiService.GetAsync<List<ChamadoDto>>("/api/chamados");

                if (chamados == null)
                {
                    chamados = new List<ChamadoDto>();
                }

                // Filtrar por período
                chamados = chamados.Where(c =>
                    c.DataAbertura >= dataInicio.Value &&
                    c.DataAbertura <= dataFim.Value
                ).ToList();

                // 1. Chamados por Status
                var chamadosPorStatus = chamados
                    .GroupBy(c => c.Status.Nome)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Total = g.Count()
                    })
                    .OrderByDescending(x => x.Total)
                    .ToList();

                // 2. Chamados por Categoria
                var chamadosPorCategoria = chamados
                    .GroupBy(c => c.Categoria.Nome)
                    .Select(g => new
                    {
                        Categoria = g.Key,
                        Total = g.Count()
                    })
                    .OrderByDescending(x => x.Total)
                    .ToList();

                // 3. Chamados por Prioridade
                var chamadosPorPrioridade = chamados
                    .GroupBy(c => c.Prioridade.Nivel)
                    .Select(g => new
                    {
                        Prioridade = g.Key,
                        Total = g.Count()
                    })
                    .OrderBy(x => Array.IndexOf(OrdemPrioridades, x.Prioridade)) // da mais grave para a mais leve
                    .ToList();

                // 4. Performance de Técnicos
                var performanceTecnicos = chamados
                    .Where(c => c.UsuarioTecnico != null)
                    .GroupBy(c => new { c.UsuarioTecnico!.IdUsuario, c.UsuarioTecnico.Nome })
                    .Select(g => new
                    {
                        Tecnico = g.Key.Nome,
                        TotalAtendidos = g.Count(),
                        Resolvidos = g.Count(c => c.Status.Nome == "Resolvido" || c.Status.Nome == "Fechado"),
                        EmAndamento = g.Count(c => c.Status.Nome == "Em Andamento" || c.Status.Nome == "Em Análise"),
                        TaxaResolucao = g.Any() ? Math.Round((double)g.Count(c => c.Status.Nome == "Resolvido" || c.Status.Nome == "Fechado") / g.Count() * 100, 1) : 0
                    })
                    .OrderByDescending(x => x.TotalAtendidos)
                    .ToList();

                // 5. Evolução Temporal (por dia)
                var evolucaoTemporal = chamados
                    .GroupBy(c => c.DataAbertura.Date)
                    .OrderBy(g => g.Key) // ordena pela data, não pelo texto "dd/MM"
                    .Select(g => new
                    {
                        Data = g.Key.ToString("dd/MM"),
                        Total = g.Count()
                    })
                    .ToList();

                // 6. Métricas Gerais
                var totalChamados = chamados.Count;
                var chamadosAbertos = chamados.Count(c => c.Status.Nome == "Aberto");
                var chamadosResolvidos = chamados.Count(c => c.Status.Nome == "Resolvido" || c.Status.Nome == "Fechado");
                var chamadosNaoAtribuidos = chamados.Count(c => c.UsuarioTecnico == null);
                var taxaResolucaoGeral = totalChamados > 0 ? Math.Round((double)chamadosResolvidos / totalChamados * 100, 1) : 0;

                // Passar dados para a view
                ViewData["ChamadosPorStatus"] = System.Text.Json.JsonSerializer.Serialize(chamadosPorStatus);
                ViewData["ChamadosPorCategoria"] = System.Text.Json.JsonSerializer.Serialize(chamadosPorCategoria);
                ViewData["ChamadosPorPrioridade"] = System.Text.Json.JsonSerializer.Serialize(chamadosPorPrioridade);
                ViewData["PerformanceTecnicos"] = performanceTecnicos;
                ViewData["EvolucaoTemporal"] = System.Text.Json.JsonSerializer.Serialize(evolucaoTemporal);

                ViewData["TotalChamados"] = totalChamados;
                ViewData["ChamadosAbertos"] = chamadosAbertos;
                ViewData["ChamadosResolvidos"] = chamadosResolvidos;
                ViewData["ChamadosNaoAtribuidos"] = chamadosNaoAtribuidos;
                ViewData["TaxaResolucaoGeral"] = taxaResolucaoGeral;

                ViewData["DataInicio"] = dataInicio.Value.ToString("yyyy-MM-dd");
                ViewData["DataFim"] = dataFim.Value.ToString("yyyy-MM-dd");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao carregar relatórios: {ex.Message}");
                TempData["Erro"] = "Erro ao carregar relatórios";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
