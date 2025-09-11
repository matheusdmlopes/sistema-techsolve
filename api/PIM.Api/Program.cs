using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Logging básico padrão do ASP.NET já configurado
var connectionString = builder.Configuration.GetConnectionString("Default");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Configuração do Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// CORS simples - permite tudo (projeto acadêmico)
builder.Services.AddCors();

// Validação automática padrão do ASP.NET mantida



var app = builder.Build();

// Aplicar migrations automaticamente na inicialização
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Aplica todas as migrations pendentes automaticamente
        context.Database.Migrate();
        app.Logger.LogInformation("Migrations aplicadas com sucesso!");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Erro ao aplicar migrations: {Message}", ex.Message);
    }
}

// Middleware removido - try/catch simples nos controllers

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS simples
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapControllers();

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