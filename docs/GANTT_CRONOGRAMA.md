# Diagrama de Gantt - Cronograma PIM IV

## Sistema TechSolve - Cronograma Visual (26/08/2024 - 17/11/2024)

```mermaid
gantt
    title Cronograma PIM IV - Sistema TechSolve
    dateFormat YYYY-MM-DD

    section Fase 1: Planejamento
    Configuração Inicial           :done, f1s1, 2024-08-26, 7d
    Requisitos e Modelagem         :done, f1s2, 2024-09-02, 7d

    section Fase 2: Arquitetura
    Arquitetura do Sistema         :done, f2s1, 2024-09-09, 7d
    Setup Projeto Backend          :done, f2s2, 2024-09-16, 7d

    section Fase 3: Backend
    Autenticação e Usuários        :done, f3s1, 2024-09-23, 7d
    Chamados e IA                  :done, f3s2, 2024-09-30, 7d
    Funcionalidades Avançadas      :done, f3s3, 2024-10-07, 7d

    section Fase 4: Frontend
    Interfaces Web e Desktop       :active, f4s1, 2024-10-14, 7d
    Mobile e Finalização           :f4s2, 2024-10-21, 7d

    section Fase 5: Documentação
    Documentação ABNT (Paralela)   :active, doc, 2024-10-07, 39d

    section Fase 6: Entrega
    Entrega Final UNIP             :milestone, entrega, 2024-11-17, 1d
```

---

## Diagrama de Gantt Detalhado por Tarefas

```mermaid
gantt
    title Cronograma Detalhado - Sistema TechSolve (26/08 a 17/11)
    dateFormat YYYY-MM-DD
    axisFormat %d/%m

    section Fundação
    Formação do Grupo              :done, t1, 2024-08-26, 2d
    Config. Ambiente               :done, t2, 2024-08-28, 3d
    Repositório Git                :done, t3, 2024-08-29, 2d
    Requisitos                     :done, t4, 2024-09-02, 3d
    Diagrama ER                    :done, t5, 2024-09-04, 3d
    Wireframes                     :done, t6, 2024-09-05, 3d

    section Arquitetura
    Arquitetura em Camadas         :done, a1, 2024-09-09, 4d
    Diagramas UML                  :done, a2, 2024-09-11, 4d
    Projeto .NET 8 API             :done, a3, 2024-09-16, 3d
    Entity Framework               :done, a4, 2024-09-18, 2d
    Azure SQL Database             :done, a5, 2024-09-20, 2d

    section Backend API
    Autenticação JWT               :done, b1, 2024-09-23, 3d
    Autorização Roles              :done, b2, 2024-09-25, 2d
    CRUD Usuários                  :done, b3, 2024-09-26, 2d
    CRUD Chamados                  :done, b4, 2024-09-30, 3d
    Histórico Automático           :done, b5, 2024-10-02, 2d
    Anexos Upload/Download         :done, b6, 2024-10-03, 2d
    Integração OpenAI              :done, b7, 2024-10-04, 3d
    Sistema Comentários            :done, b8, 2024-10-07, 2d
    Feedback e Avaliação           :done, b9, 2024-10-08, 2d
    Relatórios Gerenciais          :done, b10, 2024-10-09, 2d
    Dashboard KPIs                 :done, b11, 2024-10-10, 2d
    Funcionalidades LGPD           :done, b12, 2024-10-11, 2d
    Deploy Azure                   :done, b13, 2024-10-12, 1d

    section Frontend
    Projeto Web ASP.NET            :active, f1, 2024-10-14, 3d
    ApiService HttpClient          :active, f2, 2024-10-15, 2d
    Login e Registro Web           :active, f3, 2024-10-16, 2d
    Dashboard Web                  :active, f4, 2024-10-17, 3d
    Listagem Chamados Web          :active, f5, 2024-10-18, 2d
    Projeto Desktop WinForms       :active, f6, 2024-10-19, 2d
    Telas Desktop                  :active, f7, 2024-10-20, 2d
    Projeto Mobile Android         :f8, 2024-10-21, 2d
    Telas Autenticação Mobile      :f9, 2024-10-22, 2d
    Listagem Chamados Mobile       :f10, 2024-10-23, 2d
    Detalhes e Abertura Mobile     :f11, 2024-10-24, 3d

    section Documentação
    Estrutura ABNT                 :active, d1, 2024-10-07, 5d
    Introdução e Metodologia       :active, d2, 2024-10-15, 7d
    Arquitetura e UML              :active, d3, 2024-10-22, 7d
    Resultados e LGPD              :d4, 2024-10-29, 7d
    Manuais de Usuário             :d5, 2024-11-05, 5d
    Apresentação PowerPoint        :d6, 2024-11-08, 4d
    Revisão Final ABNT             :d7, 2024-11-12, 3d

    section Entrega
    Empacotamento Final            :crit, e1, 2024-11-15, 2d
    Postagem UNIP                  :crit, milestone, e2, 2024-11-17, 1d
```

---

## Marcos do Projeto (Milestones)

```mermaid
gantt
    title Marcos Principais - Sistema TechSolve
    dateFormat YYYY-MM-DD

    section Marcos
    M1: Formação do Grupo          :done, milestone, m1, 2024-08-30, 1d
    M2: Requisitos e Modelagem     :done, milestone, m2, 2024-09-08, 1d
    M3: Arquitetura e UML          :done, milestone, m3, 2024-09-22, 1d
    M4: API Funcional              :done, milestone, m4, 2024-10-13, 1d
    M5: Sistema Completo (3 UI)    :active, milestone, m5, 2024-10-27, 1d
    M6: Documentação ABNT          :milestone, m6, 2024-11-15, 1d
    M7: ENTREGA FINAL UNIP         :crit, milestone, m7, 2024-11-17, 1d
```

---

## Como Visualizar o Diagrama

### Opção 1: GitHub
Cole este arquivo no GitHub e os diagramas Mermaid serão renderizados automaticamente.

### Opção 2: Mermaid Live Editor
1. Acesse: https://mermaid.live/
2. Cole o código Mermaid
3. Visualize e exporte como PNG/SVG

### Opção 3: VS Code
1. Instale a extensão "Markdown Preview Mermaid Support"
2. Abra este arquivo e use a preview do Markdown

### Opção 4: Confluence/Notion
Ambas as plataformas suportam diagramas Mermaid nativamente.

---

**Legenda:**
- ✅ Verde (done): Tarefas concluídas
- 🔵 Azul (active): Tarefas em andamento
- ⚪ Branco: Tarefas pendentes
- 🔴 Vermelho (crit): Tarefas críticas
- 💎 Diamante (milestone): Marcos importantes

---

**Gerado em:** 21/10/2024
**Projeto:** Sistema TechSolve - PIM IV
**Prazo Final:** 17/11/2024
