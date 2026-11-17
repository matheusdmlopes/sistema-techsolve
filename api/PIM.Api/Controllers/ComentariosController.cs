using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;
using PIM.Api.DTOs.Requests;
using PIM.Api.DTOs.Responses;
using PIM.Api.Entities;

namespace PIM.Api.Controllers;

[ApiController]
[Route("api/chamados/{idChamado}/comentarios")]
[Authorize]
public class ComentariosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ComentariosController> _logger;

    public ComentariosController(ApplicationDbContext context, ILogger<ComentariosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os comentários de um chamado
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ComentarioDto>>> GetComentarios(int idChamado)
    {
        try
        {
            var chamadoExiste = await _context.Chamados.AnyAsync(c => c.IdChamado == idChamado);
            if (!chamadoExiste)
            {
                return NotFound(new { erro = "Chamado não encontrado" });
            }

            var comentarios = await _context.ComentariosChamados
                .Where(c => c.IdChamado == idChamado)
                .Include(c => c.Usuario)
                    .ThenInclude(u => u.PerfilAcesso)
                .OrderBy(c => c.DataHoraEnvio)
                .Select(c => new ComentarioDto
                {
                    IdComentario = c.IdComentario,
                    IdChamado = c.IdChamado,
                    IdUsuario = c.IdUsuario,
                    NomeUsuario = c.Usuario.NomeCompleto,
                    Mensagem = c.Mensagem,
                    DataHoraEnvio = c.DataHoraEnvio,
                    Lido = c.Lido,
                    EhTecnico = c.Usuario.PerfilAcesso.NomePerfil == "Técnico" || c.Usuario.PerfilAcesso.NomePerfil == "Administrador"
                })
                .ToListAsync();

            return Ok(comentarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar comentários do chamado {IdChamado}", idChamado);
            return StatusCode(500, new { erro = "Erro ao buscar comentários", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Envia um novo comentário (usado como fallback se SignalR falhar)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ComentarioDto>> EnviarComentario(int idChamado, [FromBody] EnviarComentarioRequest request)
    {
        try
        {
            var chamadoExiste = await _context.Chamados.AnyAsync(c => c.IdChamado == idChamado);
            if (!chamadoExiste)
            {
                return NotFound(new { erro = "Chamado não encontrado" });
            }

            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int idUsuario))
            {
                return Unauthorized(new { erro = "Usuário não autenticado" });
            }

            var comentario = new ComentarioChamado
            {
                IdChamado = idChamado,
                IdUsuario = idUsuario,
                Mensagem = request.Mensagem,
                DataHoraEnvio = DateTime.Now,
                Lido = false
            };

            _context.ComentariosChamados.Add(comentario);
            await _context.SaveChangesAsync();

            var usuario = await _context.Usuarios
                .Include(u => u.PerfilAcesso)
                .FirstAsync(u => u.IdUsuario == idUsuario);

            var comentarioDto = new ComentarioDto
            {
                IdComentario = comentario.IdComentario,
                IdChamado = comentario.IdChamado,
                IdUsuario = comentario.IdUsuario,
                NomeUsuario = usuario.NomeCompleto,
                Mensagem = comentario.Mensagem,
                DataHoraEnvio = comentario.DataHoraEnvio,
                Lido = comentario.Lido,
                EhTecnico = usuario.PerfilAcesso.NomePerfil == "Técnico" || usuario.PerfilAcesso.NomePerfil == "Administrador"
            };

            return CreatedAtAction(nameof(GetComentarios), new { idChamado }, comentarioDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar comentário");
            return StatusCode(500, new { erro = "Erro ao enviar comentário", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Marca comentários como lidos
    /// </summary>
    [HttpPut("marcar-lidos")]
    public async Task<IActionResult> MarcarComoLidos(int idChamado)
    {
        try
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int idUsuario))
            {
                return Unauthorized(new { erro = "Usuário não autenticado" });
            }

            var comentariosNaoLidos = await _context.ComentariosChamados
                .Where(c => c.IdChamado == idChamado && c.IdUsuario != idUsuario && !c.Lido)
                .ToListAsync();

            foreach (var comentario in comentariosNaoLidos)
            {
                comentario.Lido = true;
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensagem = $"{comentariosNaoLidos.Count} comentários marcados como lidos" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao marcar comentários como lidos");
            return StatusCode(500, new { erro = "Erro ao marcar comentários", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Conta quantos comentários não lidos o usuário tem
    /// </summary>
    [HttpGet("/api/comentarios/nao-lidos")]
    public async Task<ActionResult<int>> ContarNaoLidos()
    {
        try
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int idUsuario))
            {
                return Unauthorized(new { erro = "Usuário não autenticado" });
            }

            // Buscar chamados onde o usuário está envolvido (criador ou técnico)
            var chamadosDoUsuario = await _context.Chamados
                .Where(c => c.IdUsuarioCriador == idUsuario || c.IdUsuarioTecnico == idUsuario)
                .Select(c => c.IdChamado)
                .ToListAsync();

            // Contar comentários não lidos nesses chamados (que não foram enviados pelo próprio usuário)
            var naoLidos = await _context.ComentariosChamados
                .Where(c => chamadosDoUsuario.Contains(c.IdChamado) && c.IdUsuario != idUsuario && !c.Lido)
                .CountAsync();

            return Ok(new { total = naoLidos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao contar comentários não lidos");
            return StatusCode(500, new { erro = "Erro ao contar comentários", detalhes = ex.Message });
        }
    }
}
