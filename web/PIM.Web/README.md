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
├── Controllers/
│   ├── AuthController.cs         # Autenticação
│   ├── ChamadosController.cs     # Gestão de chamados
│   └── HomeController.cs         # Dashboard
├── Models/
│   ├── LoginViewModel.cs
│   ├── LoginResponseDto.cs
│   ├── ChamadoDto.cs             # DTOs de chamados
│   ├── ChamadoViewModels.cs      # ViewModels de chamados
│   └── DashboardDto.cs           # Métricas do dashboard
├── Views/
│   ├── Auth/
│   │   └── Login.cshtml
│   ├── Chamados/
│   │   ├── Index.cshtml          # Lista de chamados
│   │   ├── Novo.cshtml           # Formulário novo chamado
│   │   └── Detalhes.cshtml       # Detalhes completos
│   ├── Home/
│   │   └── Index.cshtml          # Dashboard integrado
│   └── Shared/
│       └── _Layout.cshtml
├── Services/
│   └── ApiService.cs             # Cliente HTTP genérico
├── Filters/
│   └── AuthorizeSessionAttribute.cs
├── wwwroot/
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

### ✅ Sistema de Autenticação
- ✅ Login/Logout de usuários
- ✅ Sessão com token JWT
- ✅ Filtro de autorização customizado (`[AuthorizeSession]`)
- ✅ Controle de perfis (Admin, Técnico, Solicitante)

### ✅ Dashboard
- ✅ Métricas em tempo real da API
- ✅ Cards com estatísticas:
  - Chamados Abertos
  - Chamados Em Atendimento
  - Chamados Resolvidos
  - Total de Chamados
  - Técnicos Ativos
  - Tempo Médio de Resolução
- ✅ Lista de chamados recentes
- ✅ Layout responsivo com Bootstrap

### ✅ Módulo de Chamados - SOLICITANTES
- ✅ **Listar Meus Chamados** - Visualização filtrada dos próprios chamados
- ✅ **Abrir Novo Chamado** - Formulário completo com:
  - Título e descrição detalhada
  - Seleção de categoria
  - Seleção de prioridade
  - Validações de formulário
- ✅ **Visualizar Detalhes** - Página completa com:
  - Informações do chamado
  - Status e prioridade visual
  - Dados do solicitante e técnico
  - Datas de abertura/fechamento
- ✅ **Enviar Feedback** - Sistema de avaliação com:
  - Nota de 1 a 5 estrelas
  - Comentário opcional
  - Apenas para chamados resolvidos

### ✅ Módulo de Chamados - TÉCNICOS
- ✅ **Listar Todos Chamados** - Visualização completa com filtros
- ✅ **Assumir Chamado** - Atribuir chamado a si mesmo
- ✅ **Alterar Status** - Modal para mudança de status:
  - Seleção de novo status
  - Campo de observação
  - Campo de solução final
  - Histórico automático
- ✅ **Visualizar Histórico** - Timeline de alterações com:
  - Data e hora de cada mudança
  - Técnico responsável
  - Status anterior e novo
  - Observações registradas

### ✅ Recursos Avançados
- ✅ **Histórico de Chamados** - Auditoria completa de alterações
- ✅ **Visualização de Anexos** - Lista de arquivos anexados
- ✅ **Sugestões de IA** - Exibição de sugestões geradas pela IA
- ✅ **Sistema de Feedback** - Avaliação de atendimento
- ✅ **Badges visuais** - Cores diferenciadas para status e prioridades
- ✅ **Responsividade** - Layout adaptável para mobile

### 🔜 Próximos Passos (Opcional)

- Upload de anexos
- CRUD de Usuários (admin)
- Filtros avançados e busca
- Paginação de listagens
- Exportação de relatórios
- Notificações em tempo real
- Gráficos e estatísticas avançadas

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

### Autenticação
- `POST /api/auth/login` - Autenticação de usuários

### Dashboard
- `GET /api/dashboard/metricas` - Métricas gerais do sistema

### Chamados
- `GET /api/chamados` - Listar todos chamados
- `GET /api/chamados/{id}` - Obter detalhes completos
- `POST /api/chamados` - Criar novo chamado
- `PUT /api/chamados/{id}/atribuir` - Técnico assume chamado
- `PUT /api/chamados/{id}/status` - Alterar status do chamado
- `POST /api/chamados/{id}/feedback` - Enviar feedback

### Listas de Apoio
- `GET /api/categorias` - Listar categorias
- `GET /api/prioridades` - Listar prioridades

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
