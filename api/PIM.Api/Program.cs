using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using PIM.Api.Data;
using PIM.Api.Services;

// A pasta de anexos precisa existir antes de a aplicação subir; sem ela, o upload de arquivos falha.
Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"));

var builder = WebApplication.CreateBuilder(args);

// Logging básico padrão do ASP.NET já configurado

// Banco de dados SQLite: um único arquivo, criado na pasta da aplicação.
// Caminho relativo é resolvido a partir da pasta da aplicação, não de onde ela foi iniciada.
var sqlite = new SqliteConnectionStringBuilder(
    builder.Configuration.GetConnectionString("Default") ?? "Data Source=techsolve.db");
if (!Path.IsPathRooted(sqlite.DataSource))
{
    sqlite.DataSource = Path.Combine(builder.Environment.ContentRootPath, sqlite.DataSource);
}
var connectionString = sqlite.ToString();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sistema TechSolve API",
        Version = "v1",
        Description = "API REST para gestão de chamados e suporte técnico com IA"
    });

    // Configuração para autenticação JWT no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddControllers();

// Configuração do Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// SignalR para chat em tempo real
builder.Services.AddSignalR();

// CORS liberado para qualquer origem, com credenciais (necessário para o SignalR).
// A aplicação roda em rede local, então o site pode ser aberto por localhost ou pelo IP da máquina.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

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

// Serviço de IA: base de conhecimento local (padrão, funciona sem internet) ou OpenAI.
// Para usar a OpenAI, defina "IA:Modo" como "OpenAI" e informe a chave em "OpenAI:ApiKey".
var atrasoIASimulada = builder.Configuration.GetValue("IA:AtrasoSimuladoMs", 1200);
var chaveOpenAI = builder.Configuration["OpenAI:ApiKey"];
var usarOpenAI = string.Equals(builder.Configuration["IA:Modo"], "OpenAI", StringComparison.OrdinalIgnoreCase)
    && ServicoIAOpenAI.ChaveValida(chaveOpenAI);

builder.Services.AddSingleton<IServicoIA>(provedor => usarOpenAI
    ? new ServicoIAComReserva(
        new ServicoIAOpenAI(chaveOpenAI!),
        new ServicoIASimulada(),
        provedor.GetRequiredService<ILogger<ServicoIAComReserva>>())
    : new ServicoIASimulada(atrasoIASimulada));

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
        var incluirDadosDeExemplo = app.Configuration.GetValue("Demo:DadosDeExemplo", true);
        await DataSeeder.SeedAsync(context, incluirDadosDeExemplo);
        app.Logger.LogInformation("Dados iniciais verificados/criados com sucesso!");
        app.Logger.LogInformation("Banco de dados: {Arquivo}", sqlite.DataSource);
        app.Logger.LogInformation("Serviço de IA: {Modo}", usarOpenAI ? "OpenAI (com base local de reserva)" : "base de conhecimento local");
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

// Sem redirecionamento para HTTPS: a aplicação roda em rede local, sem certificado.

// CORS
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SignalR Hub
app.MapHub<PIM.Api.Hubs.ChatHub>("/chatHub");

app.MapGet("/health", () =>
{
    return "API Sistema TechSolve - Online";
})
.WithName("HealthCheck")
.WithOpenApi();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();