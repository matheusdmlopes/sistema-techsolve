# Documentação Técnica - Sistema TechSolve

Este diretório contém a documentação técnica completa do Sistema TechSolve, gerada automaticamente usando **DocFX** a partir dos comentários XML do código-fonte.

## 📚 Sobre a Documentação

A documentação foi gerada para atender ao **Item 03 (1,5 pontos)** do PIM IV, que exige:
> "Documentação do código-fonte, se possível, utilizando a ferramenta Doxygen ou similar."

Utilizamos **DocFX**, a ferramenta oficial da Microsoft para documentação de projetos .NET, que gera documentação HTML profissional a partir de comentários XML.

## 🗂️ Estrutura da Documentação

A documentação cobre:

### **API (Backend)**
- **Controllers**: ChamadosController, AuthController, IAPreliminarController
- **Entities**: Chamado, Usuario, StatusChamado
- **DTOs**: CriarChamadoRequest, AnalisePreliminarDto

### **Web (Frontend)**
- **Controllers**: ChamadosController, RelatoriosController, InstitucionalController
- **Models**: ViewModels e DTOs

## 🚀 Como Visualizar a Documentação

### Opção 1: Abrir HTML Localmente

1. Navegue até a pasta `_site`:
   ```bash
   cd _site
   ```

2. Abra o arquivo `index.html` em seu navegador:
   ```bash
   # No Linux
   xdg-open index.html

   # No Windows
   start index.html

   # No macOS
   open index.html
   ```

### Opção 2: Servidor Local (Recomendado)

Para visualizar com navegação completa e busca funcionando:

```bash
# Instalar DocFX (se ainda não tiver)
dotnet tool install -g docfx

# Servir a documentação localmente
docfx serve _site
```

Acesse: http://localhost:8080

## 🔄 Regenerar Documentação

Se você fizer alterações no código e quiser atualizar a documentação:

```bash
# 1. Gerar metadados a partir do código
docfx metadata docfx.json

# 2. Construir site HTML
docfx build docfx.json

# 3. Servir localmente (opcional)
docfx serve _site
```

## 📝 Arquivos Principais

- **docfx.json**: Configuração do DocFX (projetos, templates, etc.)
- **index.md**: Página inicial da documentação
- **toc.yml**: Sumário/navegação da documentação
- **_site/**: Diretório com HTML gerado (não commitar no git)
- **api-docs/**: Metadados YAML da API
- **web-docs/**: Metadados YAML do Web

## ✨ Recursos da Documentação

- ✅ Documentação completa de todos os Controllers
- ✅ Documentação de Entities com relacionamentos
- ✅ Documentação de DTOs de Request e Response
- ✅ Códigos HTTP de resposta documentados
- ✅ Parâmetros e retornos explicados
- ✅ Regras de negócio detalhadas
- ✅ Requisitos de autenticação e autorização
- ✅ Navegação interativa
- ✅ Busca integrada

## 📦 Entregas do PIM IV

Para entregar a documentação no PIM IV:

1. **Incluir no ZIP**: Adicione a pasta `_site` no arquivo ZIP de entrega
2. **Documento Acadêmico**: Referencie a documentação na seção correspondente
3. **Instruções**: Mencione que a documentação está em `_site/index.html`

## 🛠️ Tecnologias Utilizadas

- **DocFX 2.78.3**: Gerador de documentação
- **.NET 8.0**: Framework do projeto
- **Markdig**: Parser de Markdown
- **Template Modern**: Interface moderna do DocFX

## 📄 Licença

Sistema desenvolvido para **UNIP - PIM IV**
Ano: 2025

---

**Nota**: A pasta `_site` contém arquivos gerados automaticamente e não deve ser versionada no git. Adicione `_site/` ao `.gitignore`.
