# TechSolve - Sistema de Help Desk Inteligente

<div align="center">

**Plataforma completa de gestão de suporte técnico com Inteligência Artificial**

Desenvolvido como Projeto Integrado Multidisciplinar (PIM IV)
Análise e Desenvolvimento de Sistemas - UNIP 2025/2

[Demo Online](#ambientes-de-demonstração) • [Como Executar](#como-executar-localmente) • [Documentação](#documentação)

</div>

---

## Sobre o Projeto

O TechSolve nasceu da necessidade de modernizar o atendimento de suporte técnico, substituindo processos fragmentados por e-mail e telefone por uma solução centralizada e inteligente.

**O que foi desenvolvido:**

- **Sistema completo de Help Desk** com três interfaces (Web, Mobile e Desktop) integradas a uma API REST robusta
- **Inteligência Artificial integrada** que auxilia na triagem automática de chamados, sugerindo categorias, prioridades e até possíveis soluções
- **Painel gerencial completo** com métricas em tempo real, relatórios de desempenho e análise de satisfação
- **Conformidade com LGPD** incluindo portabilidade de dados e direito ao esquecimento
- **Chat em tempo real** entre usuários e técnicos usando SignalR para comunicação instantânea

## Como Foi Construído

### Arquitetura Multi-Plataforma

O projeto foi desenvolvido seguindo uma arquitetura moderna de APIs REST, permitindo que múltiplas interfaces consumam os mesmos serviços de backend:

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Desktop   │     │     Web     │     │   Mobile    │
│  Windows    │────▶│  ASP.NET    │◀────│   Android   │
│   Forms     │     │     MVC     │     │    Java     │
└─────────────┘     └─────────────┘     └─────────────┘
                            │
                            ▼
                    ┌───────────────┐
                    │   API REST    │
                    │   .NET 8      │
                    │       C#      │
                    └───────────────┘
                            │
                    ┌───────┴────────┐
                    │                │
                    ▼                ▼
            ┌──────────────┐  ┌──────────────┐
            │  SQL Server  │  │   OpenAI     │
            │    Azure     │  │   GPT-3.5    │
            └──────────────┘  └──────────────┘
```

### Tecnologias Utilizadas

**Backend:**
- **.NET 8** - Framework principal para API REST
- **Entity Framework Core** - ORM para acesso ao banco de dados (abordagem Code-First)
- **SQL Server** - Banco de dados hospedado no Azure
- **JWT + BCrypt** - Autenticação segura com tokens e hash de senhas
- **SignalR** - Comunicação em tempo real para o chat
- **OpenAI GPT-3.5** - Motor de inteligência artificial para análise de chamados

**Frontend:**
- **ASP.NET MVC** - Aplicação web com Razor Views
- **Android (Java)** - Aplicativo mobile nativo
- **Windows Forms** - Interface desktop (em desenvolvimento)

**Infraestrutura:**
- **Azure App Service** - Hospedagem da API e Web
- **Azure SQL Database** - Banco de dados gerenciado
- **Swagger/OpenAPI** - Documentação interativa da API

## Principais Funcionalidades

### Gestão Completa de Chamados
- Abertura de tickets com anexos e descrição detalhada
- Categorização e priorização automática ou manual
- Atribuição inteligente de técnicos por especialidade
- Histórico completo de todas as alterações
- Filtros e busca avançada

### Inteligência Artificial (Diferencial do Projeto)
A integração com OpenAI GPT-3.5 oferece três recursos principais:
- **Auto-categorização**: Analisa a descrição e sugere a categoria mais adequada
- **Priorização inteligente**: Avalia a urgência automaticamente (Baixa, Média, Alta, Crítica)
- **Sugestões de solução**: Propõe passos para resolução baseados em conhecimento prévio

Todas as sugestões da IA são registradas e podem ser aceitas ou rejeitadas pelo técnico.

### Chat em Tempo Real
- Comunicação instantânea entre solicitante e técnico via SignalR
- Indicadores de "digitando..."
- Histórico de conversas salvo automaticamente
- Marcação de mensagens lidas/não lidas

### Sistema de Feedback
- Avaliação de 1 a 5 estrelas após conclusão do atendimento
- Comentários opcionais para melhoria contínua
- Relatórios de satisfação geral e por técnico
- Apenas o solicitante pode avaliar seu próprio chamado

### Dashboards e Relatórios
- **Métricas gerais**: Total de chamados, taxa de resolução, tempo médio
- **Relatórios por período**: Análise temporal (dia, semana, mês)
- **Performance de técnicos**: Chamados resolvidos, tempo médio, satisfação
- **SLA (Service Level Agreement)**: Acompanhamento de cumprimento de prazos
- **Análise por categoria**: Identificação de problemas recorrentes

### Segurança e LGPD
- **Autenticação JWT** com tokens de 24 horas
- **Senhas criptografadas** com BCrypt
- **Controle de acesso** por perfis (Admin, Técnico, Usuário)
- **Portabilidade de dados**: Usuário pode exportar todos seus dados
- **Direito ao esquecimento**: Anonimização completa de informações pessoais

## Ambientes de Demonstração

**API REST:**
- URL: `https://api-pimads-fghxhgerbkbqhsdx.brazilsouth-01.azurewebsites.net`
- Documentação: `/swagger`

**Aplicação Web:**
- URL: `https://web-pimads-bchmhxe0azgratct.brazilsouth-01.azurewebsites.net`

**Acesso de teste:**
- Email: `admin@techsolve.com`
- Senha: `admin123`

## Como Executar Localmente

### Requisitos
- .NET 8 SDK
- SQL Server
- Chave da API OpenAI ([obter aqui](https://platform.openai.com/api-keys))

### Início Rápido

1. Clone o repositório e navegue até a pasta da API:
```bash
git clone https://github.com/matheusdmlopes/sistema-techsolve.git
cd sistema-techsolve/api/PIM.Api
```

2. Configure o banco de dados e a chave OpenAI em `appsettings.Development.json`

3. Execute a aplicação:
```bash
dotnet restore
dotnet ef database update
dotnet run
```

4. Acesse `https://localhost:7095/swagger` para testar a API

> **Nota**: Na primeira execução, o sistema cria automaticamente dados iniciais incluindo perfis, setores, categorias e um usuário administrador (admin@techsolve.com / admin123)

## Estrutura da API

A API REST foi desenvolvida com 53 endpoints organizados por contexto:

| Módulo | Endpoints | Funcionalidade |
|--------|-----------|----------------|
| Autenticação | 2 | Login e validação de token |
| Usuários | 7 | Gerenciamento completo + LGPD |
| Chamados | 7 | CRUD, atribuição e histórico |
| Inteligência Artificial | 5 | Sugestões, categorização e priorização |
| Feedback | 5 | Avaliações e relatórios de satisfação |
| Anexos | 5 | Upload, download e listagem |
| Dashboard | 2 | Métricas e feed de atividades |
| Relatórios | 5 | Análises por status, categoria, período e SLA |
| Dados Auxiliares | 8 | Listas de perfis, setores, categorias, etc. |

> Toda a API está documentada com Swagger/OpenAPI disponível em `/swagger`

## Processo de Desenvolvimento

### Metodologia
O projeto foi desenvolvido seguindo boas práticas de engenharia de software:

1. **Planejamento**: Modelagem do banco de dados e definição de requisitos
2. **Arquitetura**: Estruturação em camadas (API, Models, DTOs, Services)
3. **Desenvolvimento iterativo**: Construção incremental das funcionalidades
4. **Integração contínua**: Deploy automatizado no Azure
5. **Testes manuais**: Validação via Swagger e interfaces

### Banco de Dados
Utilizamos **Entity Framework Core** com abordagem Code-First:
- Modelos definidos como classes C#
- Migrations automáticas para versionamento do schema
- Relacionamentos configurados via Fluent API
- Data Seeding para popular dados iniciais

### Destaques Técnicos
- **Arquitetura RESTful** seguindo convenções HTTP
- **Autenticação stateless** com JWT
- **Validação de dados** em múltiplas camadas
- **Tratamento de erros** consistente
- **Logging** de operações críticas
- **CORS configurado** para segurança
- **Migrations versionadas** para controle do banco

## 🎓 Objetivos Acadêmicos

Este projeto foi desenvolvido como PIM IV do curso de Análise e Desenvolvimento de Sistemas da UNIP, atendendo aos seguintes requisitos:

| Requisito | Implementação | Status |
|-----------|--------------|--------|
| Código funcional e documentado | API REST completa com Swagger | ✅ Completo |
| Múltiplas interfaces | Web (ASP.NET MVC), Mobile (Android) | ✅ Completo |
| Banco SQL Server | Azure SQL Database com EF Core | ✅ Completo |
| Integração com IA | OpenAI GPT-3.5 para triagem inteligente | ✅ Completo |
| Relatórios gerenciais | Dashboard e múltiplos relatórios | ✅ Completo |
| Conformidade LGPD | Portabilidade e esquecimento | ✅ Completo |
| Deploy em produção | Azure App Service | ✅ Completo |

## 📚 Documentação

- **CLAUDE.md**: Instruções detalhadas para desenvolvimento e arquitetura
- **README-DOCS.md**: Guia de documentação técnica com DocFX
- **Swagger UI**: Documentação interativa da API em `/swagger`
- **Código comentado**: Métodos e classes documentadas com XML comments

## 👨‍💻 Desenvolvimento

**Projeto Integrado Multidisciplinar IV**
Curso de Análise e Desenvolvimento de Sistemas
UNIP - 2025/2

---

<div align="center">

**Desenvolvido com** ❤️ **como projeto acadêmico**

</div>
