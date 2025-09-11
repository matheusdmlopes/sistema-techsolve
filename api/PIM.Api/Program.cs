using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Default");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Configuração do Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));



var app = builder.Build();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/health", () =>
{
    return "Testando API";
})
.WithName("HealthCheck")
.WithOpenApi();

app.MapGet("/teste-conexao", async () =>
{
    try
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return Results.Ok(new { mensagem = "Conexão com SQL Azure funcionando" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { erro = "Falha na conexão com o banco", detalhes = ex.Message });
    }
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();