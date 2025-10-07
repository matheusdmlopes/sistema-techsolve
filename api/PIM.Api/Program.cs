using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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

// Configuração JWT simples
var jwtKey = "ChaveSecretaSuperSegura123456789012345678901234567890";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

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

        // Popular banco com dados iniciais se necessário
        await DataSeeder.SeedAsync(context);
        app.Logger.LogInformation("Dados iniciais verificados/criados com sucesso!");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Erro ao aplicar migrations ou seeds: {Message}", ex.Message);
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () =>
{
    return "API Sistema TechSolve - Online";
})
.WithName("HealthCheck")
.WithOpenApi();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();