using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;
using PIM.Api.Entities;
using PIM.Api.Utils;

namespace PIM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os usuários (máximo 100)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios
                    .Include(u => u.Setor)
                    .Include(u => u.PerfilAcesso)
                    .Where(u => u.Ativo)
                    .Take(100)
                    .Select(u => new
                    {
                        id = u.IdUsuario,
                        nomeCompleto = u.NomeCompleto,
                        email = u.Email,
                        telefone = u.Telefone,
                        dataCadastro = u.DataCadastro,
                        ultimoLogin = u.UltimoLogin,
                        primeiroAcesso = u.PrimeiroAcesso,
                        ativo = u.Ativo,
                        setor = new
                        {
                            id = u.Setor.IdSetor,
                            nome = u.Setor.NomeSetor
                        },
                        perfil = new
                        {
                            id = u.PerfilAcesso.IdPerfil,
                            nome = u.PerfilAcesso.NomePerfil
                        }
                    })
                    .ToListAsync();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar usuários", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um usuário por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Setor)
                    .Include(u => u.PerfilAcesso)
                    .FirstOrDefaultAsync(u => u.IdUsuario == id);

                if (usuario == null)
                {
                    return NotFound(new { erro = "Usuário não encontrado" });
                }

                return Ok(new
                {
                    id = usuario.IdUsuario,
                    nomeCompleto = usuario.NomeCompleto,
                    email = usuario.Email,
                    telefone = usuario.Telefone,
                    dataCadastro = usuario.DataCadastro,
                    ultimoLogin = usuario.UltimoLogin,
                    primeiroAcesso = usuario.PrimeiroAcesso,
                    ativo = usuario.Ativo,
                    setor = new
                    {
                        id = usuario.Setor.IdSetor,
                        nome = usuario.Setor.NomeSetor
                    },
                    perfil = new
                    {
                        id = usuario.PerfilAcesso.IdPerfil,
                        nome = usuario.PerfilAcesso.NomePerfil
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar usuário", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUsuario([FromBody] CriarUsuarioRequest request)
        {
            try
            {
                // Validar se email já existe
                var emailExiste = await _context.Usuarios.AnyAsync(u => u.Email == request.Email);
                if (emailExiste)
                {
                    return BadRequest(new { erro = "Email já cadastrado" });
                }

                // Validar se setor existe
                var setorExiste = await _context.Setores.AnyAsync(s => s.IdSetor == request.IdSetor);
                if (!setorExiste)
                {
                    return BadRequest(new { erro = "Setor não encontrado" });
                }

                // Validar se perfil existe
                var perfilExiste = await _context.PerfisAcesso.AnyAsync(p => p.IdPerfil == request.IdPerfil);
                if (!perfilExiste)
                {
                    return BadRequest(new { erro = "Perfil de acesso não encontrado" });
                }

                var usuario = new Usuario
                {
                    NomeCompleto = request.NomeCompleto,
                    Email = request.Email,
                    Senha = PasswordHelper.HashPassword(request.Senha), // Hash da senha
                    Telefone = request.Telefone,
                    IdSetor = request.IdSetor,
                    IdPerfil = request.IdPerfil,
                    DataCadastro = DateTime.UtcNow,
                    PrimeiroAcesso = true,
                    Ativo = true
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, new
                {
                    id = usuario.IdUsuario,
                    nomeCompleto = usuario.NomeCompleto,
                    email = usuario.Email,
                    mensagem = "Usuário criado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao criar usuário", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] AtualizarUsuarioRequest request)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { erro = "Usuário não encontrado" });
                }

                // Atualizar campos se fornecidos
                if (!string.IsNullOrEmpty(request.NomeCompleto))
                    usuario.NomeCompleto = request.NomeCompleto;

                if (!string.IsNullOrEmpty(request.Email))
                {
                    // Verificar se o novo email já existe em outro usuário
                    var emailExiste = await _context.Usuarios.AnyAsync(u => u.Email == request.Email && u.IdUsuario != id);
                    if (emailExiste)
                    {
                        return BadRequest(new { erro = "Email já cadastrado para outro usuário" });
                    }
                    usuario.Email = request.Email;
                }

                if (!string.IsNullOrEmpty(request.Telefone))
                    usuario.Telefone = request.Telefone;

                if (!string.IsNullOrEmpty(request.Senha))
                    usuario.Senha = PasswordHelper.HashPassword(request.Senha);

                if (request.IdSetor.HasValue)
                {
                    var setorExiste = await _context.Setores.AnyAsync(s => s.IdSetor == request.IdSetor.Value);
                    if (!setorExiste)
                    {
                        return BadRequest(new { erro = "Setor não encontrado" });
                    }
                    usuario.IdSetor = request.IdSetor.Value;
                }

                if (request.IdPerfil.HasValue)
                {
                    var perfilExiste = await _context.PerfisAcesso.AnyAsync(p => p.IdPerfil == request.IdPerfil.Value);
                    if (!perfilExiste)
                    {
                        return BadRequest(new { erro = "Perfil de acesso não encontrado" });
                    }
                    usuario.IdPerfil = request.IdPerfil.Value;
                }

                if (request.PrimeiroAcesso.HasValue)
                    usuario.PrimeiroAcesso = request.PrimeiroAcesso.Value;

                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Usuário atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao atualizar usuário", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Desativa um usuário (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { erro = "Usuário não encontrado" });
                }

                // Soft delete - apenas desativa
                usuario.Ativo = false;
                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Usuário desativado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao desativar usuário", detalhes = ex.Message });
            }
        }
    }

    // DTOs para requests
    public class CriarUsuarioRequest
    {
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public int IdSetor { get; set; }
        public int IdPerfil { get; set; }
    }

    public class AtualizarUsuarioRequest
    {
        public string? NomeCompleto { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public string? Telefone { get; set; }
        public int? IdSetor { get; set; }
        public int? IdPerfil { get; set; }
        public bool? PrimeiroAcesso { get; set; }
    }
}
