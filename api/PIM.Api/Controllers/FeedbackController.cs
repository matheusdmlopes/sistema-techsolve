using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;
using PIM.Api.Entities;

namespace PIM.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class FeedbackController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FeedbackController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Criar feedback para um chamado resolvido ou fechado
    /// </summary>
    [HttpPost("chamados/{idChamado}/feedback")]
    public async Task<IActionResult> CreateFeedback(int idChamado, [FromBody] CriarFeedbackRequest request)
    {
        try
        {
            // Verificar se o chamado existe e está fechado
            var chamado = await _context.Chamados
                .Include(c => c.Status)
                .FirstOrDefaultAsync(c => c.IdChamado == idChamado);

            if (chamado == null)
            {
                return NotFound(new { erro = "Chamado não encontrado" });
            }

            // O atendimento pode ser avaliado assim que for resolvido, e também depois de fechado.
            var statusAtual = chamado.Status?.NomeStatus;
            if (statusAtual != "Resolvido" && statusAtual != "Fechado")
            {
                return BadRequest(new { erro = "Apenas chamados resolvidos ou fechados podem receber feedback" });
            }

            // Validar se o usuário logado é o solicitante do chamado
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId) || chamado.IdUsuarioCriador != int.Parse(userId))
            {
                return StatusCode(403, new { erro = "Apenas o solicitante do chamado pode deixar feedback" });
            }

            // Verificar se já existe feedback para este chamado (constraint UNIQUE no banco)
            var feedbackExistente = await _context.Feedbacks
                .AnyAsync(f => f.IdChamado == idChamado);

            if (feedbackExistente)
            {
                return BadRequest(new { erro = "Este chamado já possui feedback registrado" });
            }

            // Validar nota (1 a 5)
            if (request.Nota < 1 || request.Nota > 5)
            {
                return BadRequest(new { erro = "A nota deve estar entre 1 e 5" });
            }

            // Criar feedback
            var feedback = new Feedback
            {
                IdChamado = idChamado,
                Nota = request.Nota,
                Comentario = request.Comentario,
                DataHoraFeedback = DateTime.Now
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetFeedbackChamado),
                new { idChamado = idChamado },
                new
                {
                    id = feedback.IdFeedback,
                    idChamado = feedback.IdChamado,
                    nota = feedback.Nota,
                    comentario = feedback.Comentario,
                    dataHoraFeedback = feedback.DataHoraFeedback
                });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao criar feedback", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Buscar feedback de um chamado específico
    /// </summary>
    [HttpGet("chamados/{idChamado}/feedback")]
    public async Task<IActionResult> GetFeedbackChamado(int idChamado)
    {
        try
        {
            // Verificar se o chamado existe
            var chamadoExiste = await _context.Chamados.AnyAsync(c => c.IdChamado == idChamado);
            if (!chamadoExiste)
            {
                return NotFound(new { erro = "Chamado não encontrado" });
            }

            // Buscar feedback
            var feedback = await _context.Feedbacks
                .Where(f => f.IdChamado == idChamado)
                .Select(f => new
                {
                    id = f.IdFeedback,
                    idChamado = f.IdChamado,
                    nota = f.Nota,
                    comentario = f.Comentario,
                    dataHoraFeedback = f.DataHoraFeedback
                })
                .FirstOrDefaultAsync();

            if (feedback == null)
            {
                return NotFound(new { erro = "Feedback não encontrado para este chamado" });
            }

            return Ok(feedback);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao buscar feedback", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Listar todos os feedbacks (com paginação simples)
    /// </summary>
    [HttpGet("feedbacks")]
    [Authorize(Roles = "Administrador,Técnico")]
    public async Task<IActionResult> ListarFeedbacks([FromQuery] int? notaMinima = null)
    {
        try
        {
            var query = _context.Feedbacks
                .Include(f => f.Chamado)
                    .ThenInclude(c => c.UsuarioCriador)
                .Include(f => f.Chamado)
                    .ThenInclude(c => c.UsuarioTecnico)
                .AsQueryable();

            // Filtro opcional por nota mínima
            if (notaMinima.HasValue)
            {
                query = query.Where(f => f.Nota >= notaMinima.Value);
            }

            var feedbacks = await query
                .OrderByDescending(f => f.DataHoraFeedback)
                .Take(100)
                .Select(f => new
                {
                    id = f.IdFeedback,
                    idChamado = f.IdChamado,
                    tituloChamado = f.Chamado.Titulo,
                    nota = f.Nota,
                    comentario = f.Comentario,
                    dataHoraFeedback = f.DataHoraFeedback,
                    solicitante = f.Chamado.UsuarioCriador.NomeCompleto,
                    tecnico = f.Chamado.UsuarioTecnico != null ? f.Chamado.UsuarioTecnico.NomeCompleto : null
                })
                .ToListAsync();

            return Ok(feedbacks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao listar feedbacks", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Relatório de satisfação geral
    /// </summary>
    [HttpGet("feedbacks/relatorio")]
    [Authorize(Roles = "Administrador,Técnico")]
    public async Task<IActionResult> RelatorioSatisfacao([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
    {
        try
        {
            var query = _context.Feedbacks.AsQueryable();

            // Filtro por período
            if (dataInicio.HasValue)
            {
                query = query.Where(f => f.DataHoraFeedback >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(f => f.DataHoraFeedback <= dataFim.Value);
            }

            var feedbacks = await query.ToListAsync();

            if (!feedbacks.Any())
            {
                return Ok(new
                {
                    totalFeedbacks = 0,
                    notaMedia = 0.0,
                    distribuicaoNotas = new Dictionary<int, int>(),
                    percentualSatisfacao = 0.0,
                    mensagem = "Nenhum feedback encontrado no período especificado"
                });
            }

            // Calcular métricas
            var totalFeedbacks = feedbacks.Count;
            var notaMedia = feedbacks.Average(f => f.Nota);

            // Distribuição de notas (1 a 5)
            var distribuicaoNotas = new Dictionary<int, int>();
            for (int i = 1; i <= 5; i++)
            {
                distribuicaoNotas[i] = feedbacks.Count(f => f.Nota == i);
            }

            // Percentual de satisfação (notas 4 e 5 são consideradas satisfatórias)
            var feedbacksSatisfatorios = feedbacks.Count(f => f.Nota >= 4);
            var percentualSatisfacao = (feedbacksSatisfatorios / (double)totalFeedbacks) * 100;

            return Ok(new
            {
                totalFeedbacks,
                notaMedia = Math.Round(notaMedia, 2),
                distribuicaoNotas,
                percentualSatisfacao = Math.Round(percentualSatisfacao, 2),
                feedbacksSatisfatorios = feedbacksSatisfatorios,
                feedbacksInsatisfatorios = totalFeedbacks - feedbacksSatisfatorios,
                periodo = new
                {
                    dataInicio = dataInicio?.ToString("yyyy-MM-dd") ?? "início",
                    dataFim = dataFim?.ToString("yyyy-MM-dd") ?? "hoje"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao gerar relatório de satisfação", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Relatório de satisfação por técnico
    /// </summary>
    [HttpGet("feedbacks/relatorio/tecnicos")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RelatorioSatisfacaoPorTecnico([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
    {
        try
        {
            var query = _context.Feedbacks
                .Include(f => f.Chamado)
                    .ThenInclude(c => c.UsuarioTecnico)
                .Where(f => f.Chamado.UsuarioTecnico != null)
                .AsQueryable();

            // Filtro por período
            if (dataInicio.HasValue)
            {
                query = query.Where(f => f.DataHoraFeedback >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                query = query.Where(f => f.DataHoraFeedback <= dataFim.Value);
            }

            var feedbacks = await query.ToListAsync();

            if (!feedbacks.Any())
            {
                return Ok(new
                {
                    tecnicos = new List<object>(),
                    mensagem = "Nenhum feedback encontrado no período especificado"
                });
            }

            // Agrupar por técnico
            var relatorioTecnicos = feedbacks
                .GroupBy(f => new
                {
                    IdTecnico = f.Chamado.IdUsuarioTecnico,
                    NomeTecnico = f.Chamado.UsuarioTecnico!.NomeCompleto
                })
                .Select(g => new
                {
                    idTecnico = g.Key.IdTecnico,
                    nomeTecnico = g.Key.NomeTecnico,
                    totalFeedbacks = g.Count(),
                    notaMedia = Math.Round(g.Average(f => f.Nota), 2),
                    feedbacksSatisfatorios = g.Count(f => f.Nota >= 4),
                    percentualSatisfacao = Math.Round((g.Count(f => f.Nota >= 4) / (double)g.Count()) * 100, 2),
                    distribuicaoNotas = new Dictionary<int, int>
                    {
                        { 1, g.Count(f => f.Nota == 1) },
                        { 2, g.Count(f => f.Nota == 2) },
                        { 3, g.Count(f => f.Nota == 3) },
                        { 4, g.Count(f => f.Nota == 4) },
                        { 5, g.Count(f => f.Nota == 5) }
                    }
                })
                .OrderByDescending(t => t.notaMedia)
                .ToList();

            return Ok(new
            {
                tecnicos = relatorioTecnicos,
                periodo = new
                {
                    dataInicio = dataInicio?.ToString("yyyy-MM-dd") ?? "início",
                    dataFim = dataFim?.ToString("yyyy-MM-dd") ?? "hoje"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro ao gerar relatório de satisfação por técnico", detalhes = ex.Message });
        }
    }
}

public class CriarFeedbackRequest
{
    public int Nota { get; set; }
    public string? Comentario { get; set; }
}
