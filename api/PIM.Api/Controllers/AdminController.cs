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
        /// SEM AUTENTICAÇÃO - Use e REMOVA imediatamente após uso!
        /// Após chamar este endpoint, reinicie a API para o DataSeeder popular novamente.
        /// </summary>
        [HttpPost("reset-data")]
        public async Task<IActionResult> ResetData()
        {
            try
            {
                _logger.LogWarning("ATENÇÃO: Iniciando limpeza de todos os dados do banco!");

                // Deletar e resetar IDENTITY na ordem correta respeitando as foreign keys
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Feedback");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Anexo");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM SugestaoIA");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM HistoricoChamado");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Chamado");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Chamado', RESEED, 0)");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Usuario");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Usuario', RESEED, 0)");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM StatusChamado");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('StatusChamado', RESEED, 0)");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Prioridade");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Prioridade', RESEED, 0)");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Categoria");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Categoria', RESEED, 0)");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM Setor");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Setor', RESEED, 0)");

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM PerfilAcesso");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('PerfilAcesso', RESEED, 0)");

                _logger.LogInformation("Dados deletados e IDs resetados com sucesso! Agora o DataSeeder pode repopular.");

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
