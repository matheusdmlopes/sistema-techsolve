using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;

namespace PIM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SetoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SetoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os setores
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSetores()
        {
            try
            {
                var setores = await _context.Setores
                    .Select(s => new
                    {
                        id = s.IdSetor,
                        nome = s.NomeSetor,
                        descricao = s.Descricao
                    })
                    .ToListAsync();

                return Ok(setores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar setores", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um setor específico por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSetor(int id)
        {
            try
            {
                var setor = await _context.Setores
                    .Where(s => s.IdSetor == id)
                    .Select(s => new
                    {
                        id = s.IdSetor,
                        nome = s.NomeSetor,
                        descricao = s.Descricao
                    })
                    .FirstOrDefaultAsync();

                if (setor == null)
                {
                    return NotFound(new { erro = "Setor não encontrado" });
                }

                return Ok(setor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar setor", detalhes = ex.Message });
            }
        }
    }
}
