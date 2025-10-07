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
                        id = s.IdStatus,
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
                        id = s.IdStatus,
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
    }
}
