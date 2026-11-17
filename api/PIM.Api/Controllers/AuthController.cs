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
    /// <summary>
    /// Controller responsável pela autenticação de usuários
    /// </summary>
    /// <remarks>
    /// Gerencia o login de usuários e geração de tokens JWT.
    /// Utiliza BCrypt para verificação segura de senhas.
    /// </remarks>
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

        /// <summary>
        /// Realiza login de usuário no sistema
        /// </summary>
        /// <remarks>
        /// Valida credenciais (email e senha) e retorna um token JWT válido por 24 horas.
        /// A senha é verificada usando BCrypt.
        /// Atualiza a data do último login do usuário.
        /// Apenas usuários ativos podem fazer login.
        /// </remarks>
        /// <param name="request">Credenciais do usuário (email e senha)</param>
        /// <returns>Token JWT e informações do usuário autenticado (incluindo perfil e setor)</returns>
        /// <response code="200">Login realizado com sucesso - retorna token e dados do usuário</response>
        /// <response code="400">Email ou senha não fornecidos</response>
        /// <response code="401">Credenciais inválidas ou usuário inativo</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Senha))
                {
                    return BadRequest(new { erro = "Email e senha são obrigatórios" });
                }

                // E-mail sem diferenciar maiúsculas de minúsculas (o teclado do celular costuma iniciar com maiúscula).
                var email = request.Email.Trim().ToLower();

                var usuario = await _context.Usuarios
                    .Include(u => u.PerfilAcesso)
                    .Include(u => u.Setor)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email && u.Ativo);

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
                        idUsuario = usuario.IdUsuario,
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
        /// <remarks>
        /// Obtém dados completos do usuário logado a partir do token JWT.
        /// Inclui informações de perfil de acesso, setor, datas de cadastro e último login.
        /// Requer token JWT válido no header Authorization.
        /// </remarks>
        /// <returns>Informações completas do usuário autenticado</returns>
        /// <response code="200">Dados do usuário retornados com sucesso</response>
        /// <response code="401">Token inválido ou não fornecido</response>
        /// <response code="404">Usuário não encontrado ou inativo</response>
        /// <response code="500">Erro interno do servidor</response>
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
                    idUsuario = usuario.IdUsuario,
                    nome = usuario.NomeCompleto,
                    email = usuario.Email,
                    telefone = usuario.Telefone,
                    perfil = usuario.PerfilAcesso?.NomePerfil,
                    idPerfil = usuario.IdPerfil,
                    setor = usuario.Setor?.NomeSetor,
                    idSetor = usuario.IdSetor,
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

    /// <summary>
    /// DTO para requisição de login
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Email do usuário
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário (será verificada com BCrypt)
        /// </summary>
        public string Senha { get; set; } = string.Empty;
    }
}