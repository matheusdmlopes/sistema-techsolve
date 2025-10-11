# PIM.Web - Sistema TechSolve (ASP.NET Core MVC)

Interface web do Sistema TechSolve para UNIP PIM IV.

## 📋 Sobre

Aplicação web desenvolvida em ASP.NET Core MVC que consome a API REST do TechSolve. Permite gestão de chamados técnicos acadêmicos através de uma interface web moderna e responsiva.

## 🚀 Tecnologias

- .NET 8
- ASP.NET Core MVC
- Bootstrap 5
- jQuery
- Razor Pages

## ⚙️ Configuração

### Pré-requisitos

- .NET 8 SDK instalado
- API do TechSolve rodando (local ou Azure)

### Executar Localmente

```bash
# Navegar para a pasta web
cd web/

# Restaurar dependências
dotnet restore

# Executar a aplicação
dotnet run

# Ou com hot reload
dotnet watch
```

A aplicação estará disponível em `https://localhost:5001` (ou a porta configurada).

## 🔧 Configuração da API

A URL da API é configurada no arquivo `appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://api-pimads-fghxhgerbkbqhsdx.brazilsouth-01.azurewebsites.net"
  }
}
```

Para desenvolvimento local, você pode alterar a URL no `appsettings.Development.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7095"
  }
}
```

## 🏗️ Estrutura do Projeto

```
web/
├── Controllers/        # Controladores MVC
│   ├── AuthController.cs       # Autenticação
│   └── HomeController.cs       # Dashboard
├── Models/            # ViewModels e DTOs
│   ├── LoginViewModel.cs
│   └── LoginResponseDto.cs
├── Views/             # Views Razor
│   ├── Auth/
│   │   └── Login.cshtml
│   ├── Home/
│   │   └── Index.cshtml
│   └── Shared/
│       └── _Layout.cshtml
├── Services/          # Serviços de integração
│   └── ApiService.cs           # Cliente HTTP para API
├── Filters/           # Filtros customizados
│   └── AuthorizeSessionAttribute.cs
├── wwwroot/           # Arquivos estáticos
├── appsettings.json
└── Program.cs
```

## 🔐 Autenticação

O sistema utiliza sessões para armazenar o token JWT retornado pela API:

1. Usuário faz login em `/Auth/Login`
2. Credenciais são enviadas para `/api/auth/login` da API
3. Token JWT é armazenado na sessão
4. Token é incluído automaticamente nas requisições subsequentes
5. Filtro `[AuthorizeSession]` protege controllers que requerem autenticação

## 📝 Funcionalidades Implementadas

### ✅ Concluído

- Login/Logout de usuários
- Dashboard inicial com cards de estatísticas
- Layout responsivo com Bootstrap
- Serviço de comunicação com API (GET, POST, PUT, DELETE)
- Autenticação via JWT armazenado em sessão
- Filtro de autorização customizado

### 🔜 Próximos Passos

- CRUD de Chamados
- CRUD de Usuários
- Listagem de Categorias e Prioridades
- Visualização de histórico de chamados
- Upload de anexos
- Sistema de feedback
- Integração completa com dashboard (dados reais da API)
- Implementação de busca e filtros
- Paginação de listagens

## 🎨 Interface

### Página de Login
- Layout centralizado com gradiente roxo
- Validação de formulário
- Feedback de erros

### Dashboard
- Cards com estatísticas (em desenvolvimento)
- Menu de navegação responsivo
- Dropdown de usuário com opção de logout

## 🔗 Endpoints da API Consumidos

- `POST /api/auth/login` - Autenticação de usuários

**Próximos endpoints a serem integrados:**
- `GET /api/chamados` - Listar chamados
- `POST /api/chamados` - Criar chamado
- `GET /api/usuarios` - Listar usuários
- E outros conforme necessário

## 🐛 Debug

Para debugar a aplicação, use:

```bash
# Visual Studio Code
F5 (configurar launch.json se necessário)

# Linha de comando
dotnet run --launch-profile https
```

## 📦 Build e Deploy

### Build para produção

```bash
dotnet publish -c Release -o ./publish
```

### Deploy no Azure App Service

1. Criar um App Service no Azure (região Brazil South recomendada)
2. Configurar deploy via:
   - Azure CLI
   - GitHub Actions
   - Visual Studio
   - FTP

## 📚 Documentação Adicional

- [ASP.NET Core MVC](https://learn.microsoft.com/pt-br/aspnet/core/mvc/)
- [Razor Pages](https://learn.microsoft.com/pt-br/aspnet/core/razor-pages/)
- [Bootstrap 5](https://getbootstrap.com/docs/5.0/)

## 👥 Desenvolvedores

Projeto acadêmico - UNIP PIM IV

---

**Nota**: Este é um projeto acadêmico com arquitetura simplificada. Em produção, recomenda-se implementar:
- Cookie/session encryption
- CSRF protection avançado
- Rate limiting
- Logging estruturado
- Error handling global
- Cache de dados
