using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PIM.Api.Data;

namespace PIM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _jwtKey = "ChaveSecretaSuperSegura123456789012345678901234567890";

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Senha))
                {
                    return BadRequest(new { erro = "Email e senha são obrigatórios" });
                }

                var usuario = await _context.Usuarios
                    .Include(u => u.PerfilAcesso)
                    .Include(u => u.Setor)
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.Ativo);

                if (usuario == null)
                {
                    return Unauthorized(new { erro = "Credenciais inválidas" });
                }

                // Verificar senha com BCrypt
                if (!Utils.PasswordHelper.VerifyPassword(request.Senha, usuario.Senha))
                {
                    return Unauthorized(new { erro = "Credenciais inválidas" });
                }

                // Gerar JWT
                var token = GerarJwtToken(usuario);

                // Atualizar último login
                usuario.UltimoLogin = DateTime.Now;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    token = token,
                    usuario = new
                    {
                        id = usuario.IdUsuario,
                        nome = usuario.NomeCompleto,
                        email = usuario.Email,
                        telefone = usuario.Telefone,
                        perfil = usuario.PerfilAcesso?.NomePerfil,
                        idPerfil = usuario.IdPerfil,
                        idSetor = usuario.IdSetor,
                        setor = usuario.Setor?.NomeSetor,
                        primeiroAcesso = usuario.PrimeiroAcesso,
                        ativo = usuario.Ativo
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro interno do servidor", detalhes = ex.Message });
            }
        }

        private string GerarJwtToken(Entities.Usuario usuario)
        {
            var claims = new[]
            {
                new Claim("id", usuario.IdUsuario.ToString()),
                new Claim("email", usuario.Email),
                new Claim("nome", usuario.NomeCompleto),
                new Claim("perfil", usuario.PerfilAcesso?.NomePerfil ?? ""),
                new Claim(ClaimTypes.Role, usuario.PerfilAcesso?.NomePerfil ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddHours(24),
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Retorna informações do usuário autenticado
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { erro = "Token inválido" });
                }

                var usuario = await _context.Usuarios
                    .Include(u => u.PerfilAcesso)
                    .Include(u => u.Setor)
                    .FirstOrDefaultAsync(u => u.IdUsuario == int.Parse(userId) && u.Ativo);

                if (usuario == null)
                {
                    return NotFound(new { erro = "Usuário não encontrado" });
                }

                return Ok(new
                {
                    id = usuario.IdUsuario,
                    nome = usuario.NomeCompleto,
                    email = usuario.Email,
                    telefone = usuario.Telefone,
                    perfil = usuario.PerfilAcesso?.NomePerfil,
                    setor = usuario.Setor?.NomeSetor,
                    primeiroAcesso = usuario.PrimeiroAcesso,
                    dataCadastro = usuario.DataCadastro,
                    ultimoLogin = usuario.UltimoLogin
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro interno do servidor", detalhes = ex.Message });
            }
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}