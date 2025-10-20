using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using PIM.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Logging básico padrão do ASP.NET já configurado
var connectionString = builder.Configuration.GetConnectionString("Default");

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
    options.UseSqlServer(connectionString));

// SignalR para chat em tempo real
builder.Services.AddSignalR();

// CORS configurado para SignalR e aplicações Desktop/Mobile
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                  // Web App Azure (Produção)
                  "https://web-pimads-bchmhxe0azgratct.brazilsouth-01.azurewebsites.net",
                  // Localhost Web (Dev)
                  "http://localhost:5000",
                  "https://localhost:5001",
                  "https://localhost:7000",
                  "http://localhost:7000",
                  // Localhost API (Dev)
                  "http://localhost:5220",
                  "https://localhost:7220",
                  "https://localhost:5038",
                  "http://localhost:5038"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials() // Necessário para SignalR
              .SetIsOriginAllowedToAllowWildcardSubdomains(); // Permite subdomínios
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