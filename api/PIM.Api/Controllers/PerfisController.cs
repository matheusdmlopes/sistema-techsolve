using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;

namespace PIM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PerfisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PerfisController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os perfis de acesso
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPerfis()
        {
            try
            {
                var perfis = await _context.PerfisAcesso
                    .Select(p => new
                    {
                        id = p.IdPerfil,
                        nome = p.NomePerfil,
                        descricao = p.Descricao
                    })
                    .ToListAsync();

                return Ok(perfis);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar perfis", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um perfil específico por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerfil(int id)
        {
            try
            {
                var perfil = await _context.PerfisAcesso
                    .Where(p => p.IdPerfil == id)
                    .Select(p => new
                    {
                        id = p.IdPerfil,
                        nome = p.NomePerfil,
                        descricao = p.Descricao
                    })
                    .FirstOrDefaultAsync();

                if (perfil == null)
                {
                    return NotFound(new { erro = "Perfil não encontrado" });
                }

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar perfil", detalhes = ex.Message });
            }
        }
    }
}
