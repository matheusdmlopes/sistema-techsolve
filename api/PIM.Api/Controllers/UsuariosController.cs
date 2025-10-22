using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;
using PIM.Api.Entities;
using PIM.Api.Utils;
using System.Text.Json.Serialization;

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
        /// Lista todos os técnicos ativos
        /// </summary>
        [HttpGet("tecnicos")]
        public async Task<IActionResult> GetTecnicos()
        {
            try
            {
                var perfilTecnico = await _context.PerfisAcesso
                    .FirstOrDefaultAsync(p => p.NomePerfil == "Técnico");

                if (perfilTecnico == null)
                {
                    return Ok(new List<object>());
                }

                var tecnicos = await _context.Usuarios
                    .Where(u => u.Ativo && u.IdPerfil == perfilTecnico.IdPerfil)
                    .OrderBy(u => u.NomeCompleto)
                    .Select(u => new
                    {
                        idUsuario = u.IdUsuario,
                        nome = u.NomeCompleto,
                        email = u.Email
                    })
                    .ToListAsync();

                return Ok(tecnicos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao buscar técnicos", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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
        /// Exportar dados pessoais do usuário (LGPD - Direito de Portabilidade)
        /// </summary>
        [HttpGet("{id}/dados")]
        [Authorize]
        public async Task<IActionResult> ExportarDadosPessoais(int id)
        {
            try
            {
                // Validar se o usuário pode acessar esses dados
                var userId = User.FindFirst("id")?.Value;
                var userRole = User.FindFirst("role")?.Value;

                // Apenas o próprio usuário ou Admin pode exportar dados
                if (userRole != "Admin" && (string.IsNullOrEmpty(userId) || int.Parse(userId) != id))
                {
                    return StatusCode(403, new { erro = "Você não tem permissão para acessar esses dados" });
                }

                var usuario = await _context.Usuarios
                    .Include(u => u.Setor)
                    .Include(u => u.PerfilAcesso)
                    .FirstOrDefaultAsync(u => u.IdUsuario == id);

                if (usuario == null)
                {
                    return NotFound(new { erro = "Usuário não encontrado" });
                }

                // Chamados criados pelo usuário
                var chamadosCriados = await _context.Chamados
                    .Where(c => c.IdUsuarioCriador == id)
                    .Select(c => new
                    {
                        id = c.IdChamado,
                        titulo = c.Titulo,
                        descricao = c.Descricao,
                        dataAbertura = c.DataAbertura,
                        dataFechamento = c.DataFechamento
                    })
                    .ToListAsync();

                // Feedbacks deixados pelo usuário
                var feedbacks = await _context.Feedbacks
                    .Include(f => f.Chamado)
                    .Where(f => f.Chamado.IdUsuarioCriador == id)
                    .Select(f => new
                    {
                        idChamado = f.IdChamado,
                        tituloChamado = f.Chamado.Titulo,
                        nota = f.Nota,
                        comentario = f.Comentario,
                        dataFeedback = f.DataHoraFeedback
                    })
                    .ToListAsync();

                // Chamados atendidos (se for técnico)
                var chamadosAtendidos = await _context.Chamados
                    .Where(c => c.IdUsuarioTecnico == id)
                    .Select(c => new
                    {
                        id = c.IdChamado,
                        titulo = c.Titulo,
                        dataAbertura = c.DataAbertura,
                        dataFechamento = c.DataFechamento
                    })
                    .ToListAsync();

                var dadosPessoais = new
                {
                    informacoesPessoais = new
                    {
                        id = usuario.IdUsuario,
                        nomeCompleto = usuario.NomeCompleto,
                        email = usuario.Email,
                        telefone = usuario.Telefone,
                        setor = usuario.Setor?.NomeSetor,
                        perfil = usuario.PerfilAcesso?.NomePerfil,
                        dataCadastro = usuario.DataCadastro,
                        ultimoLogin = usuario.UltimoLogin,
                        ativo = usuario.Ativo
                    },
                    chamadosCriados = new
                    {
                        total = chamadosCriados.Count,
                        lista = chamadosCriados
                    },
                    feedbacks = new
                    {
                        total = feedbacks.Count,
                        lista = feedbacks
                    },
                    chamadosAtendidos = new
                    {
                        total = chamadosAtendidos.Count,
                        lista = chamadosAtendidos
                    },
                    dataExportacao = DateTime.UtcNow,
                    avisoLGPD = "Estes são todos os dados pessoais armazenados no sistema conforme a Lei Geral de Proteção de Dados (LGPD)"
                };

                return Ok(dadosPessoais);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao exportar dados pessoais", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Remover dados pessoais do usuário (LGPD - Direito ao Esquecimento)
        /// </summary>
        [HttpDelete("{id}/dados")]
        [Authorize]
        public async Task<IActionResult> RemoverDadosPessoais(int id)
        {
            try
            {
                // Validar se o usuário pode remover esses dados
                var userId = User.FindFirst("id")?.Value;
                var userRole = User.FindFirst("role")?.Value;

                // Apenas o próprio usuário ou Admin pode remover dados
                if (userRole != "Admin" && (string.IsNullOrEmpty(userId) || int.Parse(userId) != id))
                {
                    return StatusCode(403, new { erro = "Você não tem permissão para remover esses dados" });
                }

                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { erro = "Usuário não encontrado" });
                }

                // LGPD - Anonimizar dados ao invés de deletar completamente
                // Mantém registros para auditoria, mas remove dados pessoais identificáveis
                usuario.NomeCompleto = $"Usuário Removido #{id}";
                usuario.Email = $"removido{id}@anonimizado.lgpd";
                usuario.Telefone = null;
                usuario.Senha = PasswordHelper.HashPassword(Guid.NewGuid().ToString()); // Senha aleatória
                usuario.Ativo = false;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensagem = "Dados pessoais removidos com sucesso (anonimização LGPD)",
                    detalhes = new
                    {
                        idUsuario = id,
                        dadosAnonimizados = true,
                        dataRemocao = DateTime.UtcNow,
                        observacao = "Os dados foram anonimizados conforme LGPD. Registros de chamados foram mantidos para auditoria, mas sem informações pessoais identificáveis."
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao remover dados pessoais", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Desativa um usuário (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
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

        /// <summary>
        /// Atualiza dados pessoais do próprio usuário (nome e telefone)
        /// </summary>
        [HttpPut("{id}/perfil")]
        public async Task<IActionResult> AtualizarPerfil(int id, [FromBody] AtualizarPerfilRequest request)
        {
            try
            {
                // Validar se o usuário está atualizando o próprio perfil
                var userId = User.FindFirst("id")?.Value;
                var userRole = User.FindFirst("role")?.Value;

                if (userRole != "Admin" && (string.IsNullOrEmpty(userId) || int.Parse(userId) != id))
                {
                    return StatusCode(403, new { erro = "Você só pode atualizar seu próprio perfil" });
                }

                var usuario = await _context.Usuarios.FindAsync(id);

                if (usuario == null)
                {
                    return NotFound(new { erro = "Usuário não encontrado" });
                }

                // Atualizar apenas nome e telefone
                if (!string.IsNullOrWhiteSpace(request.NomeCompleto))
                {
                    usuario.NomeCompleto = request.NomeCompleto;
                }

                if (request.Telefone != null) // permite limpar telefone
                {
                    usuario.Telefone = request.Telefone;
                }

                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Perfil atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao atualizar perfil", detalhes = ex.Message });
            }
        }

        /// <summary>
        /// Altera a senha do usuário (para primeiro acesso ou troca de senha)
        /// </summary>
        [HttpPut("{id}/alterar-senha")]
        public async Task<IActionResult> AlterarSenha(int id, [FromBody] AlterarSenhaRequest request)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);

                if (usuario == null)
                {
                    return NotFound(new { erro = "Usuário não encontrado" });
                }

                // Verifica se a senha atual está correta
                if (!PasswordHelper.VerifyPassword(request.SenhaAtual, usuario.Senha))
                {
                    return BadRequest(new { erro = "Senha atual incorreta" });
                }

                // Valida nova senha
                if (string.IsNullOrWhiteSpace(request.NovaSenha) || request.NovaSenha.Length < 6)
                {
                    return BadRequest(new { erro = "A nova senha deve ter pelo menos 6 caracteres" });
                }

                // Atualiza a senha e marca que não é mais primeiro acesso
                usuario.Senha = PasswordHelper.HashPassword(request.NovaSenha);
                usuario.PrimeiroAcesso = false;

                await _context.SaveChangesAsync();

                return Ok(new { mensagem = "Senha alterada com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro ao alterar senha", detalhes = ex.Message });
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

    public class AtualizarPerfilRequest
    {
        [JsonPropertyName("nomeCompleto")]
        public string? NomeCompleto { get; set; }

        [JsonPropertyName("telefone")]
        public string? Telefone { get; set; }
    }

    public class AlterarSenhaRequest
    {
        [JsonPropertyName("senhaAtual")]
        public string SenhaAtual { get; set; } = string.Empty;

        [JsonPropertyName("novaSenha")]
        public string NovaSenha { get; set; } = string.Empty;
    }
}
