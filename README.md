# 🎫 Sistema TechSolve

Sistema Integrado de Gestão de Chamados e Suporte Técnico com Inteligência Artificial, desenvolvido como Projeto Integrado Multidisciplinar (PIM IV) do curso de Análise e Desenvolvimento de Sistemas da UNIP.

## 📋 Visão Geral

O TechSolve substitui o modelo tradicional de atendimento por e-mail/telefone por uma plataforma centralizada que permite:

- 📱 **Abertura de chamados** via Desktop, Web ou Mobile
- 🤖 **Triagem automática com IA** (OpenAI GPT-3.5) para categorização e sugestões de solução
- 📊 **Gestão completa** de chamados com histórico automático de alterações
- ⭐ **Feedback e avaliação** de atendimentos
- 📈 **Relatórios gerenciais** e dashboard com métricas
- 🔒 **Conformidade LGPD** (portabilidade e direito ao esquecimento)

## 🏗️ Arquitetura

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Desktop   │     │     Web     │     │   Mobile    │
│ Windows     │────▶│  ASP.NET    │◀────│   Android   │
│  Forms      │     │             │     │    Java     │
└─────────────┘     └─────────────┘     └─────────────┘
                            │
                            ▼
                    ┌───────────────┐
                    │   API REST    │
                    │   .NET 8      │
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

### Stack Tecnológico

**Backend (API REST):**
- .NET 8 Web API
- Entity Framework Core (Code-First)
- SQL Server (Azure SQL Database)
- JWT Authentication + BCrypt
- OpenAI API (GPT-3.5-turbo)

**Hospedagem:**
- Azure App Service (API)
- Azure SQL Database
- Deploy automático via Git Push

**Interfaces (em desenvolvimento):**
- Desktop: Windows Forms (C#)
- Web: ASP.NET Core (C#)
- Mobile: Android (Java)

## 🚀 Como Rodar Localmente

### Pré-requisitos

- .NET 8 SDK
- SQL Server (ou Azure SQL Database)
- Chave OpenAI API ([obter aqui](https://platform.openai.com/api-keys))

### Configuração

1. **Clone o repositório:**
```bash
git clone https://github.com/matheusdmlopes/sistema-techsolve.git
cd sistema-techsolve
```

2. **Configure a connection string:**

Edite `api/PIM.Api/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=TechSolve;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. **Configure a OpenAI API Key:**

Adicione no mesmo arquivo:
```json
{
  "OpenAI": {
    "ApiKey": "sk-proj-..."
  }
}
```

4. **Restaure as dependências e rode as migrations:**
```bash
cd api/PIM.Api
dotnet restore
dotnet ef database update
```

5. **Execute a API:**
```bash
dotnet run
```

A API estará disponível em: `https://localhost:7095`
Swagger UI: `https://localhost:7095/swagger`

6. **Compile e execute o WinForms:**
```bash
cd desktop/PIM.Desktop
xbuild PIM.Desktop.csproj /p:Configuration=Release
wine bin/Release/PIM.Desktop.exe
```

### Dados Iniciais (DataSeeder)

Ao iniciar pela primeira vez, o sistema cria automaticamente:
- Perfis de acesso (Admin, Técnico, Usuário)
- Setores (TI, RH, Financeiro, Vendas, Operações)
- Categorias (Hardware, Software, Rede, etc.)
- Prioridades (Baixa, Média, Alta, Crítica)
- Status de chamados
- **Usuário Admin padrão:**
  - Email: `admin@techsolve.com`
  - Senha: `admin123`

## 🌐 Ambientes

### Autenticação
1. POST `/api/Auth/login` com `{ "email": "admin@techsolve.com", "senha": "admin123" }`
2. Copie o `token` retornado
3. No Swagger: clique em **Authorize** e cole: `Bearer {token}`

## ✨ Funcionalidades Principais

### 🔐 Autenticação e Usuários
- Login com JWT (24h de validade)
- CRUD de usuários (Admin only)
- Perfis: Admin, Técnico, Usuário
- Validações de segurança por role

### 🎫 Gestão de Chamados
- Abertura de chamados com categoria, prioridade e status
- Atribuição automática ou manual de técnicos
- **Histórico automático** de alterações de status
- Anexos (upload/download de arquivos)
- Busca e filtros

### 🤖 Inteligência Artificial (Diferencial)
- **Auto-categorização:** IA sugere a categoria baseada na descrição
- **Auto-priorização:** IA classifica urgência (Baixa, Média, Alta, Crítica)
- **Sugestões de solução:** ChatGPT analisa e propõe passos para resolver
- Histórico de sugestões aceitas/rejeitadas

### ⭐ Feedback e Avaliação
- Apenas solicitante pode avaliar (segurança)
- Nota de 1 a 5 + comentário opcional
- Relatório de satisfação geral e por técnico

### 📊 Relatórios e Dashboard
- Métricas gerais (total, abertos, resolvidos, tempo médio)
- Relatórios por status, categoria, período
- Performance de técnicos
- SLA (Service Level Agreement)
- Agrupamento temporal (dia, semana, mês)

### 🔒 LGPD
- Senhas com hash BCrypt
- **Portabilidade:** exportação completa de dados pessoais (GET `/api/usuarios/{id}/dados`)
- **Direito ao esquecimento:** anonimização de dados (DELETE `/api/usuarios/{id}/dados`)
- Validações de autorização (apenas dono ou Admin)

## 📡 API Endpoints

### Total: 53 endpoints

- **Autenticação:** 2 endpoints (login, me)
- **Usuários:** 7 endpoints (CRUD + LGPD)
- **Chamados:** 7 endpoints (CRUD + atribuir + histórico)
- **IA:** 5 endpoints (sugestão, categorizar, priorizar)
- **Feedback:** 5 endpoints (CRUD + relatórios)
- **Anexos:** 5 endpoints (upload, download, listar)
- **Dashboard:** 2 endpoints (métricas, atividades)
- **Relatórios:** 5 endpoints (status, categoria, performance, período, SLA)
- **Listas de apoio:** 8 endpoints (perfis, setores, categorias, prioridades, status)

## 🎓 Requisitos Acadêmicos Atendidos

| Item | Descrição | Status |
|------|-----------|--------|
| ✅ | Código-fonte funcional, comentado e validado | Completo |
| ✅ | 3 Interfaces (Desktop, Web, Mobile) | Em desenvolvimento |
| ✅ | Banco MS SQL Server | Azure SQL Database |
| ✅ | Integração com IA | OpenAI GPT-3.5 |
| ✅ | Relatórios gerenciais e dashboard | Completo |
| ✅ | Conformidade LGPD | Completo |
| ✅ | Deploy em produção | Azure App Service |

## 🔧 Tecnologias Utilizadas

- **Framework:** .NET 8
- **ORM:** Entity Framework Core 8
- **Autenticação:** JWT Bearer + BCrypt
- **Banco de Dados:** SQL Server (Azure)
- **IA:** OpenAI API (GPT-3.5-turbo)
- **Documentação:** Swagger/OpenAPI
- **Hospedagem:** Azure App Service
- **Versionamento:** Git + GitHub

## 👥 Equipe

Projeto desenvolvido como PIM IV - Análise e Desenvolvimento de Sistemas - UNIP 2025/2

## 📄 Licença

Este projeto é de natureza acadêmica e foi desenvolvido para fins educacionais.
