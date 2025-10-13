using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;
using PIM.Api.Entities;
using PIM.Api.DTOs.Requests;
using PIM.Api.DTOs.Responses;

namespace PIM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChamadosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChamadosController> _logger;

        public ChamadosController(ApplicationDbContext context, ILogger<ChamadosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os chamados (máximo 100)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ChamadoDto>>> GetChamados()
        {
            try
            {
                var chamados = await _context.Chamados
                    .Include(c => c.UsuarioCriador)
                    .Include(c => c.UsuarioTecnico)
                    .Include(c => c.Categoria)
                    .Include(c => c.Prioridade)
                    .Include(c => c.Status)
                    .Take(100)
                    .OrderByDescending(c => c.DataAbertura)
                    .ToListAsync();

                var chamadosDto = chamados.Select(c => new ChamadoDto
                {
                    IdChamado = c.IdChamado,
                    Titulo = c.Titulo,
                    Descricao = c.Descricao,
                    DataAbertura = c.DataAbertura,
                    DataFechamento = c.DataFechamento,
                    SolucaoFinal = c.SolucaoFinal,
                    Categoria = new CategoriaDto
                    {
                        IdCategoria = c.Categoria.IdCategoria,
                        Nome = c.Categoria.NomeCategoria,
                        Descricao = c.Categoria.Descricao
                    },
                    Prioridade = new PrioridadeDto
                    {
                        IdPrioridade = c.Prioridade.IdPrioridade,
                        Nivel = c.Prioridade.NivelPrioridade,
                        Descricao = c.Prioridade.Descricao
                    },
                    Status = new StatusDto
                    {
                        IdStatus = c.Status.IdStatus,
                        Nome = c.Status.NomeStatus,
                        Descricao = c.Status.Descricao
                    },
                    UsuarioCriador = new UsuarioSimplificadoDto
                    {
                        IdUsuario = c.UsuarioCriador.IdUsuario,
                        Nome = c.UsuarioCriador.NomeCompleto
                    },
                    UsuarioTecnico = c.UsuarioTecnico != null ? new UsuarioSimplificadoDto
                    {
                        IdUsuario = c.UsuarioTecnico.IdUsuario,
                        Nome = c.UsuarioTecnico.NomeCompleto
                    } : null
                }).ToList();

                return Ok(chamadosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar chamados");
                return StatusCode(500, new { erro = "Erro ao buscar chamados", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um chamado por ID com todos os relacionamentos
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ChamadoDetalhadoDto>> GetChamado(int id)
        {
            try
            {
                var chamado = await _context.Chamados
                    .Include(c => c.UsuarioCriador)
                        .ThenInclude(u => u.Setor)
                    .Include(c => c.UsuarioTecnico)
                    .Include(c => c.Categoria)
                    .Include(c => c.Prioridade)
                    .Include(c => c.Status)
                    .Include(c => c.Historicos)
                        .ThenInclude(h => h.Tecnico)
                    .Include(c => c.Anexos)
                    .Include(c => c.Sugestoes)
                    .Include(c => c.Feedback)
                    .FirstOrDefaultAsync(c => c.IdChamado == id);

                if (chamado == null)
                {
                    return NotFound(new { erro = "Chamado não encontrado" });
                }

                var chamadoDto = new ChamadoDetalhadoDto
                {
                    IdChamado = chamado.IdChamado,
                    Titulo = chamado.Titulo,
                    Descricao = chamado.Descricao,
                    DataAbertura = chamado.DataAbertura,
                    DataFechamento = chamado.DataFechamento,
                    SolucaoFinal = chamado.SolucaoFinal,
                    Categoria = new CategoriaDto
                    {
                        IdCategoria = chamado.Categoria.IdCategoria,
                        Nome = chamado.Categoria.NomeCategoria,
                        Descricao = chamado.Categoria.Descricao
                    },
                    Prioridade = new PrioridadeDto
                    {
                        IdPrioridade = chamado.Prioridade.IdPrioridade,
                        Nivel = chamado.Prioridade.NivelPrioridade,
                        Descricao = chamado.Prioridade.Descricao
                    },
                    Status = new StatusDto
                    {
                        IdStatus = chamado.Status.IdStatus,
                        Nome = chamado.Status.NomeStatus,
                        Descricao = chamado.Status.Descricao
                    },
                    UsuarioCriador = new UsuarioComSetorDto
                    {
                        IdUsuario = chamado.UsuarioCriador.IdUsuario,
                        Nome = chamado.UsuarioCriador.NomeCompleto,
                        Email = chamado.UsuarioCriador.Email,
                        Setor = chamado.UsuarioCriador.Setor != null ? new SetorDto
                        {
                            IdSetor = chamado.UsuarioCriador.Setor.IdSetor,
                            Nome = chamado.UsuarioCriador.Setor.NomeSetor
                        } : null
                    },
                    UsuarioTecnico = chamado.UsuarioTecnico != null ? new UsuarioSimplificadoDto
                    {
                        IdUsuario = chamado.UsuarioTecnico.IdUsuario,
                        Nome = chamado.UsuarioTecnico.NomeCompleto
                    } : null,
                    Historicos = chamado.Historicos.Select(h => new HistoricoChamadoDto
                    {
                        IdHistorico = h.IdHistorico,
                        DataMudanca = h.DataHoraAlteracao,
                        StatusAnterior = h.StatusAnterior,
                        StatusNovo = h.StatusAtual,
                        Observacao = h.Observacao,
                        Tecnico = h.Tecnico != null ? new UsuarioSimplificadoDto
                        {
                            IdUsuario = h.Tecnico.IdUsuario,
                            Nome = h.Tecnico.NomeCompleto
                        } : new UsuarioSimplificadoDto { IdUsuario = 0, Nome = "Sistema" }
                    }).OrderByDescending(h => h.DataMudanca).ToList(),
                    Anexos = chamado.Anexos.Select(a => new AnexoDto
                    {
                        IdAnexo = a.IdAnexo,
                        NomeArquivo = a.NomeArquivo,
                        CaminhoArquivo = $"/api/anexos/{a.IdAnexo}/download",
                        DataUpload = a.DataUpload
                    }).ToList(),
                    Sugestoes = chamado.Sugestoes.Select(s => new SugestaoIADto
                    {
                        IdSugestao = s.IdSugestao,
                        TextoSugestao = s.TextoSugestao,
                        DataSugestao = s.DataHoraGeracao,
                        FoiAceita = s.FoiAceita
                    }).OrderByDescending(s => s.DataSugestao).ToList(),
                    Feedback = chamado.Feedback != null ? new FeedbackDto
                    {
                        IdFeedback = chamado.Feedback.IdFeedback,
                        Nota = chamado.Feedback.Nota,
                        Comentario = chamado.Feedback.Comentario,
                        DataFeedback = chamado.Feedback.DataHoraFeedback
                    } : null
                };

                return Ok(chamadoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar chamado {ChamadoId}", id);
                return StatusCode(500, new { erro = "Erro ao buscar chamado", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo chamado
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ChamadoCriadoDto>> CreateChamado([FromBody] CriarChamadoRequest request)
        {
            try
            {
                // Obter ID do usuário do token JWT ou do request
                var userId = User.FindFirst("id")?.Value;
                int idUsuarioCriador;

                if (request.IdUsuarioCriador.HasValue)
                {
                    idUsuarioCriador = request.IdUsuarioCriador.Value;
                }
                else if (!string.IsNullOrEmpty(userId))
                {
                    idUsuarioCriador = int.Parse(userId);
                }
                else
                {
                    return Unauthorized(new { erro = "Usuário não autenticado" });
                }

                // Validar categoria
                var categoriaExiste = await _context.Categorias.AnyAsync(c => c.IdCategoria == request.IdCategoria);
                if (!categoriaExiste)
                {
                    return BadRequest(new { erro = "Categoria não encontrada" });
                }

                // Validar prioridade
                var prioridadeExiste = await _context.Prioridades.AnyAsync(p => p.IdPrioridade == request.IdPrioridade);
                if (!prioridadeExiste)
                {
                    return BadRequest(new { erro = "Prioridade não encontrada" });
                }

                // Buscar status "Aberto"
                var statusAberto = await _context.StatusChamados.FirstOrDefaultAsync(s => s.NomeStatus == "Aberto");
                if (statusAberto == null)
                {
                    return BadRequest(new { erro = "Status 'Aberto' não encontrado no sistema" });
                }

                var chamado = new Chamado
                {
                    Titulo = request.Titulo,
                    Descricao = request.Descricao,
                    IdCategoria = request.IdCategoria,
                    IdPrioridade = request.IdPrioridade,
                    IdStatus = statusAberto.IdStatus,
                    IdUsuarioCriador = idUsuarioCriador,
                    DataAbertura = DateTime.UtcNow
                };

                _context.Chamados.Add(chamado);
                await _context.SaveChangesAsync();

                // Criar histórico inicial
                var historicoInicial = new HistoricoChamado
                {
                    IdChamado = chamado.IdChamado,
                    StatusAnterior = null,
                    StatusAtual = statusAberto.NomeStatus,
                    DataHoraAlteracao = DateTime.UtcNow,
                    Observacao = "Chamado criado"
                };

                _context.HistoricosChamados.Add(historicoInicial);
                await _context.SaveChangesAsync();

                // Buscar chamado com relacionamentos para retornar
                var chamadoCriado = await _context.Chamados
                    .Include(c => c.Categoria)
                    .Include(c => c.Prioridade)
                    .Include(c => c.Status)
                    .Include(c => c.UsuarioCriador)
                    .FirstOrDefaultAsync(c => c.IdChamado == chamado.IdChamado);

                var responseDto = new ChamadoDto
                {
                    IdChamado = chamadoCriado!.IdChamado,
                    Titulo = chamadoCriado.Titulo,
                    Descricao = chamadoCriado.Descricao,
                    DataAbertura = chamadoCriado.DataAbertura,
                    Categoria = new CategoriaDto
                    {
                        IdCategoria = chamadoCriado.Categoria.IdCategoria,
                        Nome = chamadoCriado.Categoria.NomeCategoria
                    },
                    Prioridade = new PrioridadeDto
                    {
                        IdPrioridade = chamadoCriado.Prioridade.IdPrioridade,
                        Nivel = chamadoCriado.Prioridade.NivelPrioridade
                    },
                    Status = new StatusDto
                    {
                        IdStatus = chamadoCriado.Status.IdStatus,
                        Nome = chamadoCriado.Status.NomeStatus
                    },
                    UsuarioCriador = new UsuarioSimplificadoDto
                    {
                        IdUsuario = chamadoCriado.UsuarioCriador.IdUsuario,
                        Nome = chamadoCriado.UsuarioCriador.NomeCompleto
                    }
                };

                return CreatedAtAction(nameof(GetChamado), new { id = chamado.IdChamado }, responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar chamado");
                return StatusCode(500, new { erro = "Erro ao criar chamado", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza o status de um chamado e registra no histórico
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Técnico")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] AtualizarStatusRequest request)
        {
            try
            {
                var chamado = await _context.Chamados
                    .Include(c => c.Status)
                    .FirstOrDefaultAsync(c => c.IdChamado == id);

                if (chamado == null)
                {
                    return NotFound(new { erro = "Chamado não encontrado" });
                }

                // Validar novo status
                var novoStatus = await _context.StatusChamados.FindAsync(request.IdStatus);
                if (novoStatus == null)
                {
                    return BadRequest(new { erro = "Status não encontrado" });
                }

                // Obter ID do técnico do token
                var userId = User.FindFirst("id")?.Value;
                var statusAnterior = chamado.Status.NomeStatus;

                // Atualizar status
                chamado.IdStatus = request.IdStatus;

                // Se status for "Fechado" ou "Resolvido", registrar data de fechamento
                if (novoStatus.NomeStatus == "Fechado" || novoStatus.NomeStatus == "Resolvido")
                {
                    chamado.DataFechamento = DateTime.UtcNow;
                    if (!string.IsNullOrEmpty(request.SolucaoFinal))
                    {
                        chamado.SolucaoFinal = request.SolucaoFinal;
                    }
                }

                // Atribuir técnico se fornecido
                if (request.IdUsuarioTecnico.HasValue)
                {
                    var tecnicoExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == request.IdUsuarioTecnico.Value);
                    if (!tecnicoExiste)
                    {
                        return BadRequest(new { erro = "Técnico não encontrado" });
                    }
                    chamado.IdUsuarioTecnico = request.IdUsuarioTecnico.Value;
                }

                // Criar histórico
                var historico = new HistoricoChamado
                {
                    IdChamado = id,
                    StatusAnterior = statusAnterior,
                    StatusAtual = novoStatus.NomeStatus,
                    DataHoraAlteracao = DateTime.UtcNow,
                    Observacao = request.Observacao,
                    IdTecnico = !string.IsNullOrEmpty(userId) ? int.Parse(userId) : null
                };

                _context.HistoricosChamados.Add(historico);
                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Status atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do chamado {ChamadoId}", id);
                return StatusCode(500, new { erro = "Erro ao atualizar status", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o histórico de um chamado
        /// </summary>
        [HttpGet("{id}/historico")]
        public async Task<ActionResult<List<HistoricoChamadoDto>>> GetHistorico(int id)
        {
            try
            {
                var chamadoExiste = await _context.Chamados.AnyAsync(c => c.IdChamado == id);
                if (!chamadoExiste)
                {
                    return NotFound(new { erro = "Chamado não encontrado" });
                }

                var historico = await _context.HistoricosChamados
                    .Include(h => h.Tecnico)
                    .Where(h => h.IdChamado == id)
                    .OrderByDescending(h => h.DataHoraAlteracao)
                    .Select(h => new HistoricoChamadoDto
                    {
                        IdHistorico = h.IdHistorico,
                        DataMudanca = h.DataHoraAlteracao,
                        StatusAnterior = h.StatusAnterior,
                        StatusNovo = h.StatusAtual,
                        Observacao = h.Observacao,
                        Tecnico = h.Tecnico != null ? new UsuarioSimplificadoDto
                        {
                            IdUsuario = h.Tecnico.IdUsuario,
                            Nome = h.Tecnico.NomeCompleto
                        } : new UsuarioSimplificadoDto { IdUsuario = 0, Nome = "Sistema" }
                    })
                    .ToListAsync();

                return Ok(historico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar histórico do chamado {ChamadoId}", id);
                return StatusCode(500, new { erro = "Erro ao buscar histórico", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Atribui um técnico a um chamado
        /// </summary>
        [HttpPut("{id}/atribuir")]
        [Authorize(Roles = "Admin,Técnico")]
        public async Task<IActionResult> AtribuirTecnico(int id, [FromBody] AtribuirTecnicoRequest request)
        {
            try
            {
                var chamado = await _context.Chamados
                    .Include(c => c.Status)
                    .FirstOrDefaultAsync(c => c.IdChamado == id);

                if (chamado == null)
                {
                    return NotFound(new { erro = "Chamado não encontrado" });
                }

                var tecnicoExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == request.IdUsuarioTecnico);
                if (!tecnicoExiste)
                {
                    return BadRequest(new { erro = "Técnico não encontrado" });
                }

                var statusAnterior = chamado.Status.NomeStatus;
                chamado.IdUsuarioTecnico = request.IdUsuarioTecnico;

                // Alterar status para "Em Atendimento" se ainda estiver "Aberto"
                if (chamado.Status.NomeStatus == "Aberto")
                {
                    var statusEmAtendimento = await _context.StatusChamados
                        .FirstOrDefaultAsync(s => s.NomeStatus == "Em Atendimento");

                    if (statusEmAtendimento != null)
                    {
                        chamado.IdStatus = statusEmAtendimento.IdStatus;

                        // Registrar mudança de status no histórico
                        var historico = new HistoricoChamado
                        {
                            IdChamado = id,
                            StatusAnterior = statusAnterior,
                            StatusAtual = "Em Atendimento",
                            DataHoraAlteracao = DateTime.UtcNow,
                            Observacao = "Técnico assumiu o chamado",
                            IdTecnico = request.IdUsuarioTecnico
                        };
                        _context.HistoricosChamados.Add(historico);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Técnico atribuído com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atribuir técnico ao chamado {ChamadoId}", id);
                return StatusCode(500, new { erro = "Erro ao atribuir técnico", detalhes = ex.Message });
            }
        }
    }
}
