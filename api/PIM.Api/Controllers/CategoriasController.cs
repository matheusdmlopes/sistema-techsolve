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
                        idCategoria = c.IdCategoria,
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
                        idCategoria = c.IdCategoria,
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

        /// <summary>
        /// Cria uma nova categoria
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCategoria([FromBody] CreateCategoriaRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nome))
                {
                    return BadRequest(new { erro = "O nome da categoria é obrigatório" });
                }

                var categoria = new Entities.Categoria
                {
                    NomeCategoria = request.Nome.Trim(),
                    Descricao = request.Descricao?.Trim()
                };

                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();

                var response = new
                {
                    idCategoria = categoria.IdCategoria,
                    nome = categoria.NomeCategoria,
                    descricao = categoria.Descricao
                };

                return CreatedAtAction(nameof(GetCategoria), new { id = categoria.IdCategoria }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao criar categoria", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma categoria existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoria(int id, [FromBody] UpdateCategoriaRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nome))
                {
                    return BadRequest(new { erro = "O nome da categoria é obrigatório" });
                }

                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria == null)
                {
                    return NotFound(new { erro = "Categoria não encontrada" });
                }

                categoria.NomeCategoria = request.Nome.Trim();
                categoria.Descricao = request.Descricao?.Trim();

                await _context.SaveChangesAsync();

                var response = new
                {
                    idCategoria = categoria.IdCategoria,
                    nome = categoria.NomeCategoria,
                    descricao = categoria.Descricao
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao atualizar categoria", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Deleta uma categoria
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria == null)
                {
                    return NotFound(new { erro = "Categoria não encontrada" });
                }

                // Verificar se há chamados vinculados
                var chamadosVinculados = await _context.Chamados
                    .CountAsync(c => c.IdCategoria == id);

                if (chamadosVinculados > 0)
                {
                    return BadRequest(new
                    {
                        erro = $"Não é possível excluir a categoria pois existem {chamadosVinculados} chamado(s) vinculado(s) a ela"
                    });
                }

                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Categoria excluída com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao deletar categoria", detalhes = ex.Message });
            }
        }
    }

    // DTOs para request
    public class CreateCategoriaRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    public class UpdateCategoriaRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }
}
