using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;

namespace PIM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// ENDPOINT TEMPORÁRIO - Limpa todos os dados do banco para repopular com DataSeeder
        /// REMOVER APÓS USO EM PRODUÇÃO!
        /// </summary>
        [HttpPost("reset-data")]
        public async Task<IActionResult> ResetData()
        {
            try
            {
                _logger.LogWarning("ATENÇÃO: Iniciando limpeza de todos os dados do banco!");

                // Deletar na ordem correta respeitando as foreign keys
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Chamados");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Usuarios");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM StatusChamado");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Prioridades");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Categorias");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Setores");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM PerfisAcesso");

                _logger.LogInformation("Dados deletados com sucesso! Agora o DataSeeder pode repopular.");

                return Ok(new
                {
                    sucesso = true,
                    mensagem = "Todos os dados foram deletados com sucesso! Reinicie a aplicação para o DataSeeder repopular."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao limpar dados do banco");
                return StatusCode(500, new
                {
                    sucesso = false,
                    mensagem = "Erro ao limpar dados",
                    erro = ex.Message
                });
            }
        }
    }
}
