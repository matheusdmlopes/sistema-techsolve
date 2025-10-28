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

        /// <summary>
        /// Cria um novo setor
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateSetor([FromBody] CreateSetorRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nome))
                {
                    return BadRequest(new { erro = "O nome do setor é obrigatório" });
                }

                var setor = new Entities.Setor
                {
                    NomeSetor = request.Nome.Trim(),
                    Descricao = request.Descricao?.Trim()
                };

                _context.Setores.Add(setor);
                await _context.SaveChangesAsync();

                var response = new
                {
                    id = setor.IdSetor,
                    nome = setor.NomeSetor,
                    descricao = setor.Descricao
                };

                return CreatedAtAction(nameof(GetSetor), new { id = setor.IdSetor }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao criar setor", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um setor existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSetor(int id, [FromBody] UpdateSetorRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nome))
                {
                    return BadRequest(new { erro = "O nome do setor é obrigatório" });
                }

                var setor = await _context.Setores.FindAsync(id);
                if (setor == null)
                {
                    return NotFound(new { erro = "Setor não encontrado" });
                }

                setor.NomeSetor = request.Nome.Trim();
                setor.Descricao = request.Descricao?.Trim();

                await _context.SaveChangesAsync();

                var response = new
                {
                    id = setor.IdSetor,
                    nome = setor.NomeSetor,
                    descricao = setor.Descricao
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao atualizar setor", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um setor
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSetor(int id)
        {
            try
            {
                var setor = await _context.Setores.FindAsync(id);
                if (setor == null)
                {
                    return NotFound(new { erro = "Setor não encontrado" });
                }

                // Verificar se há usuários vinculados
                var usuariosVinculados = await _context.Usuarios
                    .CountAsync(u => u.IdSetor == id);

                if (usuariosVinculados > 0)
                {
                    return BadRequest(new
                    {
                        erro = $"Não é possível excluir o setor pois existem {usuariosVinculados} usuário(s) vinculado(s) a ele"
                    });
                }

                _context.Setores.Remove(setor);
                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Setor excluído com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao deletar setor", detalhes = ex.Message });
            }
        }
    }

    // DTOs para request
    public class CreateSetorRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    public class UpdateSetorRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }
}
