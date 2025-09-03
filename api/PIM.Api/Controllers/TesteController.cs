using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class TesteController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public TesteController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    [HttpGet]
    public async Task<ActionResult<List<TesteConexaoDto>>> GetTestes()
    {
        var testes = new List<TesteConexaoDto>();
        try
        {
            var connectionString = _configuration.GetConnectionString("Default");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "SELECT Id, Nome, DataCriacao, Ativo FROM TesteConexao";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var teste = new TesteConexaoDto
                {
                    Id = reader.GetInt32("Id"),
                    Nome = reader.GetString("Nome"),
                    DataCriacao = reader.GetDateTime("DataCriacao"),
                    Ativo = reader.GetBoolean("Ativo")
                };
                testes.Add(teste);
            }


            return Ok(testes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = "Falha na conexão com o banco", detalhes = ex.Message });
        }
    }
}