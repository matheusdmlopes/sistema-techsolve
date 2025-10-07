using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;
using PIM.Api.Entities;

namespace PIM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChamadosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChamadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os chamados (máximo 100)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetChamados()
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
                    .Select(c => new
                    {
                        id = c.IdChamado,
                        titulo = c.Titulo,
                        descricao = c.Descricao,
                        dataAbertura = c.DataAbertura,
                        dataFechamento = c.DataFechamento,
                        categoria = new
                        {
                            id = c.Categoria.IdCategoria,
                            nome = c.Categoria.NomeCategoria
                        },
                        prioridade = new
                        {
                            id = c.Prioridade.IdPrioridade,
                            nivel = c.Prioridade.NivelPrioridade
                        },
                        status = new
                        {
                            id = c.Status.IdStatus,
                            nome = c.Status.NomeStatus
                        },
                        usuarioCriador = new
                        {
                            id = c.UsuarioCriador.IdUsuario,
                            nome = c.UsuarioCriador.NomeCompleto
                        },
                        usuarioTecnico = c.UsuarioTecnico != null ? new
                        {
                            id = c.UsuarioTecnico.IdUsuario,
                            nome = c.UsuarioTecnico.NomeCompleto
                        } : null
                    })
                    .OrderByDescending(c => c.dataAbertura)
                    .ToListAsync();

                return Ok(chamados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar chamados", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um chamado por ID com todos os relacionamentos
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChamado(int id)
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

                return Ok(new
                {
                    id = chamado.IdChamado,
                    titulo = chamado.Titulo,
                    descricao = chamado.Descricao,
                    dataAbertura = chamado.DataAbertura,
                    dataFechamento = chamado.DataFechamento,
                    solucaoFinal = chamado.SolucaoFinal,
                    categoria = new
                    {
                        id = chamado.Categoria.IdCategoria,
                        nome = chamado.Categoria.NomeCategoria
                    },
                    prioridade = new
                    {
                        id = chamado.Prioridade.IdPrioridade,
                        nivel = chamado.Prioridade.NivelPrioridade
                    },
                    status = new
                    {
                        id = chamado.Status.IdStatus,
                        nome = chamado.Status.NomeStatus
                    },
                    usuarioCriador = new
                    {
                        id = chamado.UsuarioCriador.IdUsuario,
                        nome = chamado.UsuarioCriador.NomeCompleto,
                        email = chamado.UsuarioCriador.Email,
                        setor = chamado.UsuarioCriador.Setor.NomeSetor
                    },
                    usuarioTecnico = chamado.UsuarioTecnico != null ? new
                    {
                        id = chamado.UsuarioTecnico.IdUsuario,
                        nome = chamado.UsuarioTecnico.NomeCompleto,
                        email = chamado.UsuarioTecnico.Email
                    } : null,
                    historico = chamado.Historicos.Select(h => new
                    {
                        id = h.IdHistorico,
                        statusAnterior = h.StatusAnterior,
                        statusAtual = h.StatusAtual,
                        dataHora = h.DataHoraAlteracao,
                        observacao = h.Observacao,
                        tecnico = h.Tecnico != null ? h.Tecnico.NomeCompleto : null
                    }).OrderByDescending(h => h.dataHora),
                    anexos = chamado.Anexos.Select(a => new
                    {
                        id = a.IdAnexo,
                        nomeArquivo = a.NomeArquivo,
                        dataUpload = a.DataUpload
                    }),
                    sugestoesIA = chamado.Sugestoes.Select(s => new
                    {
                        id = s.IdSugestao,
                        texto = s.TextoSugestao,
                        dataGeracao = s.DataHoraGeracao,
                        foiAceita = s.FoiAceita
                    }),
                    feedback = chamado.Feedback != null ? new
                    {
                        nota = chamado.Feedback.Nota,
                        comentario = chamado.Feedback.Comentario,
                        dataHora = chamado.Feedback.DataHoraFeedback
                    } : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar chamado", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo chamado
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateChamado([FromBody] CriarChamadoRequest request)
        {
            try
            {
                // Obter ID do usuário do token JWT
                var userId = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userId))
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

                // Buscar status "Aberto" (assumindo IdStatus = 1)
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
                    IdUsuarioCriador = int.Parse(userId),
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

                return CreatedAtAction(nameof(GetChamado), new { id = chamado.IdChamado }, new
                {
                    id = chamado.IdChamado,
                    titulo = chamado.Titulo,
                    mensagem = "Chamado criado com sucesso"
                });
            }
            catch (Exception ex)
            {
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

                // Se status for "Fechado", registrar data de fechamento
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
                return StatusCode(500, new { erro = "Erro ao atualizar status", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o histórico de um chamado
        /// </summary>
        [HttpGet("{id}/historico")]
        public async Task<IActionResult> GetHistorico(int id)
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
                    .Select(h => new
                    {
                        id = h.IdHistorico,
                        statusAnterior = h.StatusAnterior,
                        statusAtual = h.StatusAtual,
                        dataHora = h.DataHoraAlteracao,
                        observacao = h.Observacao,
                        tecnico = h.Tecnico != null ? new
                        {
                            id = h.Tecnico.IdUsuario,
                            nome = h.Tecnico.NomeCompleto
                        } : null
                    })
                    .ToListAsync();

                return Ok(historico);
            }
            catch (Exception ex)
            {
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
                var chamado = await _context.Chamados.FindAsync(id);
                if (chamado == null)
                {
                    return NotFound(new { erro = "Chamado não encontrado" });
                }

                var tecnicoExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == request.IdUsuarioTecnico);
                if (!tecnicoExiste)
                {
                    return BadRequest(new { erro = "Técnico não encontrado" });
                }

                chamado.IdUsuarioTecnico = request.IdUsuarioTecnico;
                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Técnico atribuído com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao atribuir técnico", detalhes = ex.Message });
            }
        }
    }

    // DTOs para requests
    public class CriarChamadoRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int IdCategoria { get; set; }
        public int IdPrioridade { get; set; }
    }

    public class AtualizarStatusRequest
    {
        public int IdStatus { get; set; }
        public string? Observacao { get; set; }
        public string? SolucaoFinal { get; set; }
        public int? IdUsuarioTecnico { get; set; }
    }

    public class AtribuirTecnicoRequest
    {
        public int IdUsuarioTecnico { get; set; }
    }
}
