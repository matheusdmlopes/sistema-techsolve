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
                        id = p.IdPrioridade,
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
                        id = p.IdPrioridade,
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
    }
}
