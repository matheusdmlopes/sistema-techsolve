# Sistema TechSolve - Documentação Técnica

Bem-vindo à documentação técnica do **Sistema TechSolve**, uma solução completa de gerenciamento de chamados técnicos desenvolvida para o projeto PIM IV da UNIP.

## Visão Geral

O Sistema TechSolve é uma plataforma de Service Desk que facilita o gerenciamento de solicitações de suporte técnico em ambientes acadêmicos. O sistema utiliza **Inteligência Artificial** para análise preliminar de problemas e oferece comunicação em tempo real entre técnicos e usuários.

## Arquitetura do Sistema

O projeto está dividido em dois componentes principais:

### **API** (Backend)
- **Tecnologia**: ASP.NET Core 8.0 Web API
- **Banco de Dados**: SQL Server com Entity Framework Core
- **Autenticação**: JWT (JSON Web Tokens)
- **Segurança**: BCrypt para hash de senhas
- **IA**: Integração com OpenAI GPT-3.5-turbo
- **Comunicação em Tempo Real**: SignalR

### **Web** (Frontend)
- **Tecnologia**: ASP.NET Core 8.0 MVC
- **Interface**: Bootstrap 5 + Bootstrap Icons
- **Comunicação**: SignalR para chat em tempo real
- **Hospedagem**: Azure App Service

## Principais Funcionalidades

### 🎫 Gerenciamento de Chamados
- Abertura de chamados com análise preliminar por IA
- Categorização automática (Hardware, Software, Rede, etc.)
- Priorização inteligente (Baixa, Média, Alta, Crítica)
- Atribuição de técnicos
- Histórico completo de alterações
- Anexos de arquivos
- Feedback do usuário

### 🤖 Inteligência Artificial
- Análise preliminar de problemas
- Sugestão de categoria e prioridade
- Recomendações de solução
- Indicação se o problema pode ser resolvido sem técnico

### 💬 Chat em Tempo Real
- Comunicação direta entre usuário e técnico
- WebSocket via SignalR
- Fallback para API REST quando WebSocket não disponível
- Histórico de mensagens persistido

### 📊 Relatórios e Dashboards
- Chamados por status, categoria e prioridade
- Performance de técnicos (taxa de resolução)
- Evolução temporal de chamados
- Métricas gerais do sistema

### 👥 Gestão de Usuários
- Três perfis de acesso: Administrador, Técnico e Solicitante
- Controle de permissões por perfil
- Gerenciamento de setores

### 📚 Base de Conhecimento
- Artigos informativos
- Categorização de conteúdo
- Visualização de artigos relacionados

## Conformidade e Segurança

- **LGPD**: Política de Privacidade completa
- **Autenticação**: JWT com expiração de 24 horas
- **Criptografia**: BCrypt para senhas
- **HTTPS**: Comunicação segura em produção

## Tecnologias Utilizadas

- **.NET 8.0**
- **Entity Framework Core**
- **SQL Server**
- **SignalR**
- **OpenAI API**
- **Bootstrap 5**
- **Azure App Service**
- **GitHub Actions** (CI/CD)

## Navegação da Documentação

Utilize o menu lateral para navegar entre:

- **API Reference**: Documentação completa dos Controllers, Entities e DTOs da API
- **Web Application**: Documentação dos Controllers e Models da aplicação Web

## Contato e Suporte

Sistema desenvolvido para **UNIP - PIM IV**
Ano: 2025
