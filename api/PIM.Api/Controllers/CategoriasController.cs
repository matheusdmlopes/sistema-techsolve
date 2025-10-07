using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;

namespace PIM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todas as categorias
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            try
            {
                var categorias = await _context.Categorias
                    .Select(c => new
                    {
                        id = c.IdCategoria,
                        nome = c.NomeCategoria,
                        descricao = c.Descricao
                    })
                    .ToListAsync();

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar categorias", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma categoria específica por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias
                    .Where(c => c.IdCategoria == id)
                    .Select(c => new
                    {
                        id = c.IdCategoria,
                        nome = c.NomeCategoria,
                        descricao = c.Descricao
                    })
                    .FirstOrDefaultAsync();

                if (categoria == null)
                {
                    return NotFound(new { erro = "Categoria não encontrada" });
                }

                return Ok(categoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar categoria", detalhes = ex.Message });
            }
        }
    }
}
