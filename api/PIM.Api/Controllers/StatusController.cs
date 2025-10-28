using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;

namespace PIM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os status de chamados
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetStatus()
        {
            try
            {
                var status = await _context.StatusChamados
                    .Select(s => new
                    {
                        idStatus = s.IdStatus,
                        nome = s.NomeStatus,
                        descricao = s.Descricao
                    })
                    .ToListAsync();

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar status", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um status específico por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatusById(int id)
        {
            try
            {
                var status = await _context.StatusChamados
                    .Where(s => s.IdStatus == id)
                    .Select(s => new
                    {
                        idStatus = s.IdStatus,
                        nome = s.NomeStatus,
                        descricao = s.Descricao
                    })
                    .FirstOrDefaultAsync();

                if (status == null)
                {
                    return NotFound(new { erro = "Status não encontrado" });
                }

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar status", detalhes = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateStatus([FromBody] CreateStatusRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nome))
                {
                    return BadRequest(new { erro = "O nome do status é obrigatório" });
                }

                var status = new Entities.StatusChamado
                {
                    NomeStatus = request.Nome.Trim(),
                    Descricao = request.Descricao?.Trim()
                };

                _context.StatusChamados.Add(status);
                await _context.SaveChangesAsync();

                var response = new
                {
                    idStatus = status.IdStatus,
                    nome = status.NomeStatus,
                    descricao = status.Descricao
                };

                return CreatedAtAction(nameof(GetStatusById), new { id = status.IdStatus }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao criar status", detalhes = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nome))
                {
                    return BadRequest(new { erro = "O nome do status é obrigatório" });
                }

                var status = await _context.StatusChamados.FindAsync(id);
                if (status == null)
                {
                    return NotFound(new { erro = "Status não encontrado" });
                }

                status.NomeStatus = request.Nome.Trim();
                status.Descricao = request.Descricao?.Trim();

                await _context.SaveChangesAsync();

                var response = new
                {
                    idStatus = status.IdStatus,
                    nome = status.NomeStatus,
                    descricao = status.Descricao
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao atualizar status", detalhes = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            try
            {
                var status = await _context.StatusChamados.FindAsync(id);
                if (status == null)
                {
                    return NotFound(new { erro = "Status não encontrado" });
                }

                var chamadosVinculados = await _context.Chamados
                    .CountAsync(c => c.IdStatus == id);

                if (chamadosVinculados > 0)
                {
                    return BadRequest(new
                    {
                        erro = $"Não é possível excluir o status pois existem {chamadosVinculados} chamado(s) vinculado(s) a ele"
                    });
                }

                _context.StatusChamados.Remove(status);
                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Status excluído com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao deletar status", detalhes = ex.Message });
            }
        }
    }

    public class CreateStatusRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    public class UpdateStatusRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }
}
