using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;

namespace PIM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Técnico")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Métricas gerais do sistema para dashboard
    /// </summary>
    [HttpGet("metricas")]
    public async Task<IActionResult> GetMetricas([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
    {
        try
        {
            // Período padrão: últimos 30 dias
            var inicio = dataInicio ?? DateTime.Now.AddDays(-30);
            var fim = dataFim ?? DateTime.Now;

            // Total de chamados (geral)
            var totalChamados = await _context.Chamados.CountAsync();

            // Chamados no período
            var chamadosPeriodo = await _context.Chamados
                .Where(c => c.DataAbertura >= inicio && c.DataAbertura <= fim)
                .ToListAsync();

            // Chamados por status (atuais)
            var chamadosAbertos = await _context.Chamados
                .Include(c => c.Status)
                .Where(c => c.Status != null && c.Status.NomeStatus == "Aberto")
                .CountAsync();

            var chamadosEmAndamento = await _context.Chamados
                .Include(c => c.Status)
                .Where(c => c.Status != null &&
                    (c.Status.NomeStatus == "Em Andamento" ||
                     c.Status.NomeStatus == "Em Análise" ||
                     c.Status.NomeStatus == "Aguardando Usuário"))
                .CountAsync();

            var chamadosResolvidos = await _context.Chamados
                .Include(c => c.Status)
                .Where(c => c.Status != null && c.Status.NomeStatus == "Resolvido")
                .CountAsync();

            var chamadosFechados = await _context.Chamados
                .Include(c => c.Status)
                .Where(c => c.Status != null && c.Status.NomeStatus == "Fechado")
                .CountAsync();

            // Técnicos ativos (que têm chamados atribuídos atualmente)
            var tecnicosAtivos = await _context.Chamados
                .Where(c => c.IdUsuarioTecnico != null)
                .Select(c => c.IdUsuarioTecnico)
                .Distinct()
                .CountAsync();

            // Tempo médio de resolução (em horas)
            var chamadosFinalizados = await _context.Chamados
                .Where(c => c.DataFechamento != null)
                .Select(c => new
                {
                    c.DataAbertura,
                    c.DataFechamento
                })
                .ToListAsync();

            double tempoMedioResolucaoHoras = 0;
            if (chamadosFinalizados.Any())
            {
                var temposTotais = chamadosFinalizados
                    .Select(c => (c.DataFechamento!.Value - c.DataAbertura).TotalHours)
                    .ToList();
                tempoMedioResolucaoHoras = temposTotais.Average();
            }

            // Satisfação média (feedbacks)
            var feedbacks = await _context.Feedbacks.ToListAsync();
            double satisfacaoMedia = feedbacks.Any() ? feedbacks.Average(f => f.Nota) : 0;

            // Chamados por prioridade (ativos)
            var chamadosPorPrioridade = await _context.Chamados
                .Include(c => c.Prioridade)
                .Include(c => c.Status)
                .Where(c => c.Status != null && c.Status.NomeStatus != "Fechado" && c.Status.NomeStatus != "Cancelado")
                .GroupBy(c => c.Prioridade!.NivelPrioridade)
                .Select(g => new
                {
                    prioridade = g.Key,
                    total = g.Count()
                })
                .OrderByDescending(x => x.total)
                .ToListAsync();

            // Top 3 categorias mais solicitadas no período
            var topCategorias = await _context.Chamados
                .Where(c => c.DataAbertura >= inicio && c.DataAbertura <= fim)
                .Include(c => c.Categoria)
                .GroupBy(c => c.Categoria!.NomeCategoria)
                .Select(g => new
                {
                    categoria = g.Key,
                    total = g.Count()
                })
                .OrderByDescending(x => x.total)
                .Take(3)
                .ToListAsync();

            // Tendência (comparação com período anterior)
            var periodoAnteriorInicio = inicio.AddDays(-(fim - inicio).Days);
            var chamadosPeriodoAnterior = await _context.Chamados
                .Where(c => c.DataAbertura >= periodoAnteriorInicio && c.DataAbertura < inicio)
                .CountAsync();

            var tendencia = chamadosPeriodo.Count - chamadosPeriodoAnterior;
            var percentualTendencia = chamadosPeriodoAnterior > 0
                ? Math.Round((tendencia / (double)chamadosPeriodoAnterior) * 100, 2)
                : 0;

            return Ok(new
            {
                resumoGeral = new
                {
                    totalChamados,
                    chamadosAbertos,
                    chamadosEmAndamento,
                    chamadosResolvidos,
                    chamadosFechados,
                    tecnicosAtivos,
                    tempoMedioResolucao = new
                    {
                        horas = Math.Round(tempoMedioResolucaoHoras, 2),
                        formatado = FormatarTempoResolucao(tempoMedioResolucaoHoras)
                    },
                    satisfacaoMedia = Math.Round(satisfacaoMedia, 2),
                    totalFeedbacks = feedbacks.Count
                },
                periodo = new
                {
                    dataInicio = inicio.ToString("yyyy-MM-dd"),
                    dataFim = fim.ToString("yyyy-MM-dd"),
                    totalChamadosPeriodo = chamadosPeriodo.Count,
                    tendencia = new
                    {
                        diferenca = tendencia,
                        percentual = percentualTendencia,
                        direcao = tendencia > 0 ? "aumento" : tendencia < 0 ? "redução" : "estável"
                    }
                },
                distribuicao = new
                {
                    porPrioridade = chamadosPorPrioridade,
                    topCategorias
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao obter métricas do dashboard", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Atividade recente do sistema
    /// </summary>
    [HttpGet("atividades-recentes")]
    public async Task<IActionResult> GetAtividadesRecentes([FromQuery] int limite = 10)
    {
        try
        {
            // Últimos chamados criados
            var ultimosChamados = await _context.Chamados
                .Include(c => c.UsuarioCriador)
                .Include(c => c.Status)
                .Include(c => c.Categoria)
                .OrderByDescending(c => c.DataAbertura)
                .Take(limite)
                .Select(c => new
                {
                    tipo = "chamado_criado",
                    idChamado = c.IdChamado,
                    titulo = c.Titulo,
                    status = c.Status!.NomeStatus,
                    categoria = c.Categoria!.NomeCategoria,
                    usuario = c.UsuarioCriador.NomeCompleto,
                    dataHora = c.DataAbertura
                })
                .ToListAsync();

            // Últimas mudanças de status
            var ultimasAlteracoes = await _context.HistoricosChamados
                .Include(h => h.Tecnico)
                .Include(h => h.Chamado)
                .OrderByDescending(h => h.DataHoraAlteracao)
                .Take(limite)
                .Select(h => new
                {
                    tipo = "status_alterado",
                    idChamado = h.IdChamado,
                    titulo = h.Chamado.Titulo,
                    statusAnterior = h.StatusAnterior,
                    statusAtual = h.StatusAtual,
                    tecnico = h.Tecnico != null ? h.Tecnico.NomeCompleto : null,
                    observacao = h.Observacao,
                    dataHora = h.DataHoraAlteracao
                })
                .ToListAsync();

            // Últimos feedbacks
            var ultimosFeedbacks = await _context.Feedbacks
                .Include(f => f.Chamado)
                    .ThenInclude(c => c.UsuarioCriador)
                .OrderByDescending(f => f.DataHoraFeedback)
                .Take(limite)
                .Select(f => new
                {
                    tipo = "feedback_recebido",
                    idChamado = f.IdChamado,
                    titulo = f.Chamado.Titulo,
                    nota = f.Nota,
                    comentario = f.Comentario,
                    usuario = f.Chamado.UsuarioCriador.NomeCompleto,
                    dataHora = f.DataHoraFeedback
                })
                .ToListAsync();

            // Mesclar e ordenar todas as atividades
            var todasAtividades = new List<object>();
            todasAtividades.AddRange(ultimosChamados);
            todasAtividades.AddRange(ultimasAlteracoes);
            todasAtividades.AddRange(ultimosFeedbacks);

            var atividadesOrdenadas = todasAtividades
                .OrderByDescending(a => ((dynamic)a).dataHora)
                .Take(limite)
                .ToList();

            return Ok(new
            {
                total = atividadesOrdenadas.Count,
                atividades = atividadesOrdenadas
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao obter atividades recentes", detalhes = ex.Message });
        }
    }

    private string FormatarTempoResolucao(double horas)
    {
        if (horas < 1)
        {
            return $"{Math.Round(horas * 60)} minutos";
        }
        else if (horas < 24)
        {
            return $"{Math.Round(horas, 1)} horas";
        }
        else
        {
            var dias = Math.Floor(horas / 24);
            var horasRestantes = Math.Round(horas % 24, 1);
            return $"{dias} dia(s) e {horasRestantes} hora(s)";
        }
    }
}
