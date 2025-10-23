# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TechSolve is a full-stack help desk and technical support management system with AI integration, developed as an academic project (PIM IV). The system replaces traditional email/phone support with a centralized platform featuring:

- Multi-platform support (API, Web, Mobile)
- AI-powered ticket triaging using OpenAI GPT-3.5
- Real-time chat via SignalR
- LGPD compliance (Brazilian data protection regulation)
- Comprehensive ticket management with automated history tracking

## Architecture

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Desktop   │     │     Web     │     │   Mobile    │
│  (Planned)  │────▶│  ASP.NET    │◀────│  Android    │
│             │     │    MVC      │     │   (Early)   │
└─────────────┘     └─────────────┘     └─────────────┘
                            │
                            ▼
                    ┌───────────────┐
                    │   REST API    │
                    │   .NET 8      │
                    │  + SignalR    │
                    └───────────────┘
                            │
                    ┌───────┴────────┐
                    │                │
                    ▼                ▼
            ┌──────────────┐  ┌──────────────┐
            │  SQL Server  │  │   OpenAI     │
            │  (Azure)     │  │   GPT-3.5    │
            └──────────────┘  └──────────────┘
```

### Technology Stack

**API (api/PIM.Api):**
- .NET 8 Web API
- Entity Framework Core (Code-First)
- JWT Authentication + BCrypt password hashing
- SignalR for real-time chat
- OpenAI API integration (GPT-3.5-turbo)
- Swagger/OpenAPI documentation

**Web (web/PIM.Web):**
- ASP.NET Core MVC (.NET 8)
- Session-based auth (stores JWT token)
- ApiService for HTTP client communication with API
- Razor views with Bootstrap

**Mobile (mobile/):**
- Android app (Java)
- Gradle build system
- Early development stage

**Database:**
- SQL Server / Azure SQL Database
- Entity Framework migrations
- Auto-seeding on startup via DataSeeder

**Deployment:**
- API: Azure App Service (https://api-pimads-fghxhgerbkbqhsdx.brazilsouth-01.azurewebsites.net)
- Web: Azure App Service (https://web-pimads-bchmhxe0azgratct.brazilsouth-01.azurewebsites.net)

## Repository Structure

- **api/PIM.Api/** - REST API backend
  - Controllers/ - API endpoints (Auth, Chamados, IA, Dashboard, etc.)
  - Entities/ - Database models (Usuario, Chamado, Feedback, etc.)
  - DTOs/ - Request/Response data transfer objects
  - Data/ - ApplicationDbContext, DataSeeder, Migrations
  - Hubs/ - SignalR ChatHub for real-time messaging
  - Utils/ - Helper utilities (PasswordHelper with BCrypt)

- **web/PIM.Web/** - ASP.NET MVC frontend
  - Controllers/ - MVC controllers (Auth, Chamados, Relatorios, etc.)
  - Views/ - Razor views
  - Services/ - ApiService for REST communication
  - Models/ - ViewModels and DTOs
  - Filters/ - Authorization filters

- **mobile/** - Android app (early stage)
  - app/src/main/java/com/techsolve/app/ - Java source

- **docs/** - DocFX generated documentation

## Development Commands

### API Development

**Run the API locally:**
```bash
cd api/PIM.Api
dotnet restore
dotnet run
```
The API will be available at `https://localhost:7095` with Swagger UI at `https://localhost:7095/swagger`

**Apply database migrations:**
```bash
cd api/PIM.Api
dotnet ef database update
```

**Create new migration:**
```bash
cd api/PIM.Api
dotnet ef migrations add MigrationName
```

**Build the API:**
```bash
cd api/PIM.Api
dotnet build
```

**Run from solution root:**
```bash
dotnet build sistema-techsolve.sln
dotnet run --project api/PIM.Api/PIM.Api.csproj
```

### Web Development

**Run the Web application:**
```bash
cd web/PIM.Web
dotnet restore
dotnet run
```
The web app will be available at `https://localhost:7000` (default route: /Auth/Login)

**Build the Web app:**
```bash
cd web/PIM.Web
dotnet build
```

### Mobile Development

**Build Android app:**
```bash
cd mobile
./gradlew build
```

**Run on emulator/device:**
```bash
cd mobile
./gradlew installDebug
```

### Documentation

**Regenerate DocFX documentation:**
```bash
# From repository root
docfx metadata docfx.json
docfx build docfx.json
```

**Serve documentation locally:**
```bash
docfx serve _site
# Access at http://localhost:8080
```

## Configuration

### API Configuration (api/PIM.Api/appsettings.Development.json)

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=TechSolve;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "OpenAI": {
    "ApiKey": "sk-proj-..."
  }
}
```

**Note:** The JWT secret key is hardcoded in Program.cs:87 (academic project). The OpenAI API key should be configured in appsettings or user secrets.

### Web Configuration (web/PIM.Web/appsettings.Development.json)

For local development, update the API base URL to point to your local API:
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7095"
  }
}
```

Production uses Azure-hosted API.

## Database Schema

### Core Entities

**Usuario** (Users)
- Authentication and user management
- Relationships: Setor (Department), PerfilAcesso (Role: Admin/Tecnico/Usuario)
- BCrypt password hashing
- Tracks created and assigned tickets

**Chamado** (Tickets)
- Main ticket entity
- Related to: UsuarioCriador, UsuarioTecnico, Categoria, Prioridade, Status
- Cascade relationships: Historicos, Sugestoes, Anexos, Feedback, Comentarios

**HistoricoChamado** (Ticket History)
- Auto-generated on status changes
- Tracks status transitions and technician assignments

**SugestaoIA** (AI Suggestions)
- Stores AI-generated categorization, prioritization, and solution suggestions
- Tracks acceptance status

**Feedback** (Feedback)
- One-to-one with Chamado
- Rating (1-5) + optional comment
- Check constraint ensures rating is between 1 and 5

**ComentarioChamado** (Chat Messages)
- Real-time chat via SignalR
- Tracks read/unread status
- Indexed for performance

### Supporting Entities
- Setor (Departments)
- PerfilAcesso (Access Profiles/Roles)
- Categoria (Categories)
- Prioridade (Priorities)
- StatusChamado (Ticket Statuses)
- Anexo (File Attachments)

## Key Features & Implementation Notes

### Authentication & Authorization

**API:**
- JWT Bearer tokens (24h validity)
- Generated in AuthController
- Validated via middleware in Program.cs:88-99

**Web:**
- Session-based (stores JWT from API)
- ApiService automatically adds Bearer token to requests (ApiService.cs:33-41)
- Authorization filters in web/PIM.Web/Filters/

**Default Admin Account (seeded):**
- Email: admin@techsolve.com
- Password: admin123

### AI Integration (OpenAI GPT-3.5)

Implemented in **IAController** and **IAPreliminarController**:
- Auto-categorization: Suggests category based on ticket description
- Auto-prioritization: Suggests priority level (Baixa, Média, Alta, Crítica)
- Solution suggestions: Generates troubleshooting steps
- All AI responses are stored in SugestaoIA table

Configuration uses Azure.AI.OpenAI package (not OpenAI SDK).

### Real-Time Chat (SignalR)

**ChatHub** (api/PIM.Api/Hubs/ChatHub.cs):
- Group-based rooms per ticket: `Chamado_{idChamado}`
- Methods: EntrarNoChamado, SairDoChamado, EnviarMensagem
- Typing indicators: UsuarioDigitando, UsuarioParouDeDigitar
- Auto-saves messages to ComentarioChamado table
- Messages marked as read via MarcarMensagensComoLidas

**Endpoint:** `/chatHub` (mapped in Program.cs:147)

**CORS:** Configured in Program.cs:61-84 to support SignalR with credentials

### Data Seeding

**DataSeeder.SeedAsync()** (api/PIM.Api/Data/DataSeeder.cs):
- Automatically runs on application startup (Program.cs:118)
- Creates default:
  - Perfis de acesso (Admin, Técnico, Usuário)
  - Setores (TI, RH, Financeiro, Vendas, Operações)
  - Categorias (Hardware, Software, Rede, etc.)
  - Prioridades (Baixa, Média, Alta, Crítica)
  - Status (Aberto, Em Andamento, Resolvido, Fechado, Cancelado)
  - Admin user (admin@techsolve.com)

### LGPD Compliance

**Data Portability:** GET `/api/usuarios/{id}/dados` - exports all user data
**Right to be Forgotten:** DELETE `/api/usuarios/{id}/dados` - anonymizes user data

Both endpoints validate authorization (only owner or admin).

### API Structure

**Controllers:**
- AdminController - Admin-only operations (user management)
- AnexosController - File upload/download
- AuthController - Login, token refresh
- CategoriasController - Category management
- ChamadosController - CRUD tickets, assign technicians
- ComentariosController - Chat message operations
- DashboardController - Metrics and activity feeds
- FeedbackController - Ratings and satisfaction reports
- IAController - AI categorization and suggestions
- IAPreliminarController - Preliminary AI analysis
- PrioridadesController, SetoresController, StatusController - List endpoints
- UsuariosController - User CRUD, profile management

**Total:** 53+ endpoints documented in Swagger

### Web Application Structure

**Controllers:**
- AuthController - Login/logout, session management
- ChamadosController - Ticket views and forms
- RelatoriosController - Reports and dashboards
- UsuariosController - User management
- InstitucionalController - About/institutional pages
- PerfilController - User profile management
- HomeController - Landing page
- ArtigosController - Knowledge base articles

**ApiService Pattern:**
Session stores JWT → ApiService adds to Authorization header → Makes HTTP calls to API

### Migrations

Migrations are applied **automatically** on startup (Program.cs:114).

To create new migrations manually:
```bash
cd api/PIM.Api
dotnet ef migrations add MigrationName
```

### Testing & Quality

Currently no automated tests. Swagger UI is the primary testing tool during development.

For API testing:
1. POST `/api/Auth/login` with `{"email": "admin@techsolve.com", "senha": "admin123"}`
2. Copy the `token` from response
3. Click **Authorize** in Swagger UI
4. Enter: `Bearer {token}`

### File Structure Conventions

**DTOs:**
- api/PIM.Api/DTOs/Requests/ - Input DTOs (e.g., CriarChamadoRequest)
- api/PIM.Api/DTOs/Responses/ - Output DTOs (e.g., ChamadoResponse)

**Entity Relationships:**
- Configured in ApplicationDbContext.OnModelCreating() using Fluent API
- Navigation properties defined in entity classes

**Error Handling:**
- Controllers use try/catch blocks with appropriate HTTP status codes
- No global exception middleware (simplified for academic project)

## Common Development Patterns

### Adding a New API Endpoint

1. Define Request/Response DTOs in DTOs/Requests and DTOs/Responses
2. Add controller method with route and authorization attributes
3. Use ApplicationDbContext for data access
4. Return appropriate status codes (Ok, NotFound, BadRequest, etc.)
5. Add XML documentation comments for Swagger

### Adding a New Entity

1. Create entity class in api/PIM.Api/Entities/
2. Add DbSet to ApplicationDbContext
3. Configure entity in ApplicationDbContext.OnModelCreating()
4. Create migration: `dotnet ef migrations add AddEntityName`
5. Update DataSeeder if initial data is needed

### Adding Web Views

1. Add controller action in web/PIM.Web/Controllers/
2. Create ViewModel in Models/ if needed
3. Call ApiService methods to fetch/post data
4. Create Razor view in Views/{ControllerName}/
5. Ensure authorization filter is applied

### Working with AI Features

The AI integration uses Azure.AI.OpenAI package. When modifying:
- OpenAI API key is in appsettings (or user secrets)
- Model: "gpt-3.5-turbo"
- Temperature and max_tokens are configurable per endpoint
- All AI suggestions are logged to SugestaoIA table

## Known Issues & Considerations

- JWT secret is hardcoded (Program.cs:87) - acceptable for academic project
- No automated tests - manual testing via Swagger
- Desktop client is planned but not yet implemented
- Mobile app is in early development
- CORS is configured for specific origins - update Program.cs:65-77 for new environments
- Migrations auto-apply on startup - disable in production if needed (Program.cs:114)
- File uploads stored in wwwroot/uploads - consider blob storage for production
- SignalR requires CORS with AllowCredentials

## Azure Deployment Notes

**API Deployment:**
- Azure App Service with SQL Database connection string
- Ensure OpenAI API key is in Application Settings
- Auto-migration on startup handles database updates

**Web Deployment:**
- Update ApiSettings:BaseUrl to point to production API
- Session cookies require HTTPS in production

## Documentation Resources

- README.md - High-level project overview and setup instructions
- README-DOCS.md - DocFX documentation guide
- Swagger UI - Interactive API documentation at /swagger endpoint
- DocFX Site - Generated at _site/ (not committed to git)
