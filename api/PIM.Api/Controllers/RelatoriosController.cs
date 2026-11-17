using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;

namespace PIM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Técnico")]
public class RelatoriosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RelatoriosController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Relatório de chamados por status
    /// </summary>
    [HttpGet("chamados-status")]
    public async Task<IActionResult> ChamadosPorStatus([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
    {
        try
        {
            var query = _context.Chamados.Include(c => c.Status).AsQueryable();

            if (dataInicio.HasValue)
            {
                query = query.Where(c => c.DataAbertura >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(c => c.DataAbertura <= dataFim.Value);
            }

            var resultado = await query
                .GroupBy(c => new
                {
                    IdStatus = c.Status!.IdStatus,
                    NomeStatus = c.Status.NomeStatus
                })
                .Select(g => new
                {
                    idStatus = g.Key.IdStatus,
                    status = g.Key.NomeStatus,
                    total = g.Count(),
                    percentual = 0.0 // calculado depois
                })
                .OrderByDescending(x => x.total)
                .ToListAsync();

            var totalGeral = resultado.Sum(r => r.total);

            var resultadoComPercentual = resultado.Select(r => new
            {
                r.idStatus,
                r.status,
                r.total,
                percentual = totalGeral > 0 ? Math.Round((r.total / (double)totalGeral) * 100, 2) : 0
            }).ToList();

            return Ok(new
            {
                totalChamados = totalGeral,
                distribuicao = resultadoComPercentual,
                periodo = new
                {
                    dataInicio = dataInicio?.ToString("yyyy-MM-dd") ?? "início",
                    dataFim = dataFim?.ToString("yyyy-MM-dd") ?? "hoje"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao gerar relatório por status", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Relatório de chamados por categoria
    /// </summary>
    [HttpGet("chamados-categoria")]
    public async Task<IActionResult> ChamadosPorCategoria([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
    {
        try
        {
            var query = _context.Chamados.Include(c => c.Categoria).AsQueryable();

            if (dataInicio.HasValue)
            {
                query = query.Where(c => c.DataAbertura >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(c => c.DataAbertura <= dataFim.Value);
            }

            var resultado = await query
                .GroupBy(c => new
                {
                    IdCategoria = c.Categoria!.IdCategoria,
                    NomeCategoria = c.Categoria.NomeCategoria
                })
                .Select(g => new
                {
                    idCategoria = g.Key.IdCategoria,
                    categoria = g.Key.NomeCategoria,
                    total = g.Count()
                })
                .OrderByDescending(x => x.total)
                .ToListAsync();

            var totalGeral = resultado.Sum(r => r.total);

            var resultadoComPercentual = resultado.Select(r => new
            {
                r.idCategoria,
                r.categoria,
                r.total,
                percentual = totalGeral > 0 ? Math.Round((r.total / (double)totalGeral) * 100, 2) : 0
            }).ToList();

            return Ok(new
            {
                totalChamados = totalGeral,
                distribuicao = resultadoComPercentual,
                periodo = new
                {
                    dataInicio = dataInicio?.ToString("yyyy-MM-dd") ?? "início",
                    dataFim = dataFim?.ToString("yyyy-MM-dd") ?? "hoje"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao gerar relatório por categoria", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Relatório de performance dos técnicos
    /// </summary>
    [HttpGet("performance-tecnicos")]
    public async Task<IActionResult> PerformanceTecnicos([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
    {
        try
        {
            var query = _context.Chamados
                .Include(c => c.UsuarioTecnico)
                .Include(c => c.Status)
                .Where(c => c.IdUsuarioTecnico != null)
                .AsQueryable();

            if (dataInicio.HasValue)
            {
                query = query.Where(c => c.DataAbertura >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(c => c.DataAbertura <= dataFim.Value);
            }

            var chamados = await query.ToListAsync();

            // Feedbacks por técnico
            var feedbacksPorTecnico = await _context.Feedbacks
                .Include(f => f.Chamado)
                    .ThenInclude(c => c.UsuarioTecnico)
                .Where(f => f.Chamado.IdUsuarioTecnico != null)
                .GroupBy(f => f.Chamado.IdUsuarioTecnico)
                .Select(g => new
                {
                    IdTecnico = g.Key,
                    NotaMedia = g.Average(f => f.Nota),
                    TotalAvaliacoes = g.Count()
                })
                .ToListAsync();

            var performance = chamados
                .GroupBy(c => new
                {
                    c.IdUsuarioTecnico,
                    NomeTecnico = c.UsuarioTecnico!.NomeCompleto
                })
                .Select(g =>
                {
                    var chamadosFinalizados = g.Where(c => c.DataFechamento != null).ToList();
                    var tempoMedioResolucao = chamadosFinalizados.Any()
                        ? chamadosFinalizados.Average(c => (c.DataFechamento!.Value - c.DataAbertura).TotalHours)
                        : 0;

                    var feedback = feedbacksPorTecnico.FirstOrDefault(f => f.IdTecnico == g.Key.IdUsuarioTecnico);

                    return new
                    {
                        idTecnico = g.Key.IdUsuarioTecnico,
                        nomeTecnico = g.Key.NomeTecnico,
                        totalChamados = g.Count(),
                        chamadosAbertos = g.Count(c => c.Status!.NomeStatus == "Aberto" ||
                                                      c.Status.NomeStatus == "Em Andamento" ||
                                                      c.Status.NomeStatus == "Em Análise"),
                        chamadosResolvidos = g.Count(c => c.Status!.NomeStatus == "Resolvido"),
                        chamadosFechados = g.Count(c => c.Status!.NomeStatus == "Fechado"),
                        tempoMedioResolucaoHoras = Math.Round(tempoMedioResolucao, 2),
                        tempoMedioFormatado = FormatarTempoResolucao(tempoMedioResolucao),
                        avaliacaoMedia = feedback != null ? Math.Round(feedback.NotaMedia, 2) : 0,
                        totalAvaliacoes = feedback?.TotalAvaliacoes ?? 0,
                        taxaConclusao = g.Count() > 0
                            ? Math.Round((g.Count(c => c.Status!.NomeStatus == "Fechado") / (double)g.Count()) * 100, 2)
                            : 0
                    };
                })
                .OrderByDescending(t => t.chamadosFechados)
                .ThenByDescending(t => t.avaliacaoMedia)
                .ToList();

            return Ok(new
            {
                totalTecnicos = performance.Count,
                tecnicos = performance,
                periodo = new
                {
                    dataInicio = dataInicio?.ToString("yyyy-MM-dd") ?? "início",
                    dataFim = dataFim?.ToString("yyyy-MM-dd") ?? "hoje"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao gerar relatório de performance", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Relatório de chamados por período (diário, semanal, mensal)
    /// </summary>
    [HttpGet("chamados-periodo")]
    public async Task<IActionResult> ChamadosPorPeriodo(
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim,
        [FromQuery] string agrupamento = "dia") // dia, semana, mes
    {
        try
        {
            var chamados = await _context.Chamados
                .Where(c => c.DataAbertura >= dataInicio && c.DataAbertura <= dataFim)
                .Include(c => c.Status)
                .ToListAsync();

            if (!chamados.Any())
            {
                return Ok(new
                {
                    periodo = new { dataInicio = dataInicio.ToString("yyyy-MM-dd"), dataFim = dataFim.ToString("yyyy-MM-dd") },
                    agrupamento,
                    dados = new List<object>(),
                    mensagem = "Nenhum chamado encontrado no período"
                });
            }

            object resultado;

            switch (agrupamento.ToLower())
            {
                case "dia":
                    resultado = chamados
                        .GroupBy(c => c.DataAbertura.Date)
                        .Select(g => new
                        {
                            data = g.Key.ToString("yyyy-MM-dd"),
                            diaSemana = g.Key.ToString("dddd", new System.Globalization.CultureInfo("pt-BR")),
                            total = g.Count(),
                            abertos = g.Count(c => c.Status!.NomeStatus == "Aberto"),
                            emAndamento = g.Count(c => c.Status!.NomeStatus == "Em Andamento" || c.Status.NomeStatus == "Em Análise"),
                            fechados = g.Count(c => c.Status!.NomeStatus == "Fechado")
                        })
                        .OrderBy(x => x.data)
                        .ToList();
                    break;

                case "semana":
                    resultado = chamados
                        .GroupBy(c => new
                        {
                            Ano = c.DataAbertura.Year,
                            Semana = System.Globalization.ISOWeek.GetWeekOfYear(c.DataAbertura)
                        })
                        .Select(g => new
                        {
                            ano = g.Key.Ano,
                            semana = g.Key.Semana,
                            periodo = $"{g.Key.Ano}-S{g.Key.Semana}",
                            total = g.Count(),
                            abertos = g.Count(c => c.Status!.NomeStatus == "Aberto"),
                            emAndamento = g.Count(c => c.Status!.NomeStatus == "Em Andamento" || c.Status.NomeStatus == "Em Análise"),
                            fechados = g.Count(c => c.Status!.NomeStatus == "Fechado")
                        })
                        .OrderBy(x => x.ano).ThenBy(x => x.semana)
                        .ToList();
                    break;

                case "mes":
                    resultado = chamados
                        .GroupBy(c => new
                        {
                            Ano = c.DataAbertura.Year,
                            Mes = c.DataAbertura.Month
                        })
                        .Select(g => new
                        {
                            ano = g.Key.Ano,
                            mes = g.Key.Mes,
                            mesNome = new DateTime(g.Key.Ano, g.Key.Mes, 1).ToString("MMMM", new System.Globalization.CultureInfo("pt-BR")),
                            periodo = $"{g.Key.Ano}-{g.Key.Mes:D2}",
                            total = g.Count(),
                            abertos = g.Count(c => c.Status!.NomeStatus == "Aberto"),
                            emAndamento = g.Count(c => c.Status!.NomeStatus == "Em Andamento" || c.Status.NomeStatus == "Em Análise"),
                            fechados = g.Count(c => c.Status!.NomeStatus == "Fechado")
                        })
                        .OrderBy(x => x.ano).ThenBy(x => x.mes)
                        .ToList();
                    break;

                default:
                    return BadRequest(new { erro = "Agrupamento inválido. Use: dia, semana ou mes" });
            }

            return Ok(new
            {
                periodo = new
                {
                    dataInicio = dataInicio.ToString("yyyy-MM-dd"),
                    dataFim = dataFim.ToString("yyyy-MM-dd")
                },
                agrupamento,
                totalChamados = chamados.Count,
                dados = resultado
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao gerar relatório por período", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Relatório de SLA (tempo de atendimento)
    /// </summary>
    [HttpGet("sla")]
    public async Task<IActionResult> RelatorioSLA([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
    {
        try
        {
            var query = _context.Chamados
                .Include(c => c.Prioridade)
                .Where(c => c.DataFechamento != null)
                .AsQueryable();

            if (dataInicio.HasValue)
            {
                query = query.Where(c => c.DataAbertura >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(c => c.DataAbertura <= dataFim.Value);
            }

            var chamados = await query.ToListAsync();

            if (!chamados.Any())
            {
                return Ok(new
                {
                    mensagem = "Nenhum chamado finalizado no período",
                    periodo = new
                    {
                        dataInicio = dataInicio?.ToString("yyyy-MM-dd") ?? "início",
                        dataFim = dataFim?.ToString("yyyy-MM-dd") ?? "hoje"
                    }
                });
            }

            // SLA por prioridade (definição de metas em horas)
            var slaMetas = new Dictionary<string, double>
            {
                { "Crítica", 4 },   // 4 horas
                { "Alta", 24 },     // 24 horas
                { "Média", 48 },    // 48 horas
                { "Baixa", 120 }    // 120 horas (5 dias)
            };

            var slaPorPrioridade = chamados
                .GroupBy(c => c.Prioridade!.NivelPrioridade)
                .Select(g =>
                {
                    var tempoMedioHoras = g.Average(c => (c.DataFechamento!.Value - c.DataAbertura).TotalHours);
                    var meta = slaMetas.ContainsKey(g.Key) ? slaMetas[g.Key] : 48;
                    var dentroDaSla = g.Count(c => (c.DataFechamento!.Value - c.DataAbertura).TotalHours <= meta);

                    return new
                    {
                        prioridade = g.Key,
                        totalChamados = g.Count(),
                        tempoMedioHoras = Math.Round(tempoMedioHoras, 2),
                        tempoMedioFormatado = FormatarTempoResolucao(tempoMedioHoras),
                        metaSlaHoras = meta,
                        dentroDaSla = dentroDaSla,
                        foraDaSla = g.Count() - dentroDaSla,
                        percentualCumprimento = Math.Round((dentroDaSla / (double)g.Count()) * 100, 2)
                    };
                })
                .OrderBy(x => slaMetas.ContainsKey(x.prioridade) ? slaMetas[x.prioridade] : 999)
                .ToList();

            var totalDentroDaSla = slaPorPrioridade.Sum(s => s.dentroDaSla);
            var totalChamados = chamados.Count;

            return Ok(new
            {
                resumo = new
                {
                    totalChamados,
                    totalDentroDaSla,
                    totalForaDaSla = totalChamados - totalDentroDaSla,
                    percentualCumprimentoGeral = Math.Round((totalDentroDaSla / (double)totalChamados) * 100, 2)
                },
                porPrioridade = slaPorPrioridade,
                periodo = new
                {
                    dataInicio = dataInicio?.ToString("yyyy-MM-dd") ?? "início",
                    dataFim = dataFim?.ToString("yyyy-MM-dd") ?? "hoje"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao gerar relatório de SLA", detalhes = ex.Message });
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
