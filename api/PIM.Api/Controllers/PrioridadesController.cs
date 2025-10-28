using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;

namespace PIM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PrioridadesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PrioridadesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todas as prioridades
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPrioridades()
        {
            try
            {
                var prioridades = await _context.Prioridades
                    .Select(p => new
                    {
                        idPrioridade = p.IdPrioridade,
                        nivel = p.NivelPrioridade,
                        descricao = p.Descricao
                    })
                    .ToListAsync();

                return Ok(prioridades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar prioridades", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma prioridade específica por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrioridade(int id)
        {
            try
            {
                var prioridade = await _context.Prioridades
                    .Where(p => p.IdPrioridade == id)
                    .Select(p => new
                    {
                        idPrioridade = p.IdPrioridade,
                        nivel = p.NivelPrioridade,
                        descricao = p.Descricao
                    })
                    .FirstOrDefaultAsync();

                if (prioridade == null)
                {
                    return NotFound(new { erro = "Prioridade não encontrada" });
                }

                return Ok(prioridade);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar prioridade", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Cria uma nova prioridade
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePrioridade([FromBody] CreatePrioridadeRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nivel))
                {
                    return BadRequest(new { erro = "O nível da prioridade é obrigatório" });
                }

                var prioridade = new Entities.Prioridade
                {
                    NivelPrioridade = request.Nivel.Trim(),
                    Descricao = request.Descricao?.Trim()
                };

                _context.Prioridades.Add(prioridade);
                await _context.SaveChangesAsync();

                var response = new
                {
                    idPrioridade = prioridade.IdPrioridade,
                    nivel = prioridade.NivelPrioridade,
                    descricao = prioridade.Descricao
                };

                return CreatedAtAction(nameof(GetPrioridade), new { id = prioridade.IdPrioridade }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao criar prioridade", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma prioridade existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrioridade(int id, [FromBody] UpdatePrioridadeRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nivel))
                {
                    return BadRequest(new { erro = "O nível da prioridade é obrigatório" });
                }

                var prioridade = await _context.Prioridades.FindAsync(id);
                if (prioridade == null)
                {
                    return NotFound(new { erro = "Prioridade não encontrada" });
                }

                prioridade.NivelPrioridade = request.Nivel.Trim();
                prioridade.Descricao = request.Descricao?.Trim();

                await _context.SaveChangesAsync();

                var response = new
                {
                    idPrioridade = prioridade.IdPrioridade,
                    nivel = prioridade.NivelPrioridade,
                    descricao = prioridade.Descricao
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao atualizar prioridade", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Deleta uma prioridade
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrioridade(int id)
        {
            try
            {
                var prioridade = await _context.Prioridades.FindAsync(id);
                if (prioridade == null)
                {
                    return NotFound(new { erro = "Prioridade não encontrada" });
                }

                // Verificar se há chamados vinculados
                var chamadosVinculados = await _context.Chamados
                    .CountAsync(c => c.IdPrioridade == id);

                if (chamadosVinculados > 0)
                {
                    return BadRequest(new
                    {
                        erro = $"Não é possível excluir a prioridade pois existem {chamadosVinculados} chamado(s) vinculado(s) a ela"
                    });
                }

                _context.Prioridades.Remove(prioridade);
                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Prioridade excluída com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao deletar prioridade", detalhes = ex.Message });
            }
        }
    }

    // DTOs para request
    public class CreatePrioridadeRequest
    {
        public string Nivel { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    public class UpdatePrioridadeRequest
    {
        public string Nivel { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }
}
