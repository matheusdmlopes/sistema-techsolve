# Diagrama de Gantt - Cronograma PIM IV

## Sistema TechSolve - Cronograma Visual (26/08/2025 - 17/11/2025)

```mermaid
gantt
    title Cronograma PIM IV - Sistema TechSolve (26/08 a 17/11)
    dateFormat YYYY-MM-DD
    axisFormat %d/%m

    section Fase 1: Planejamento
    Configuração e Requisitos      :done, p1, 2025-08-26, 14d

    section Fase 2: Arquitetura
    Arquitetura e Diagramas UML    :done, p2, 2025-09-09, 7d
    Setup Backend e Banco          :done, p3, 2025-09-16, 7d

    section Fase 3: Backend
    Autenticação e Usuários        :done, b1, 2025-09-23, 7d
    Chamados e Integração IA       :done, b2, 2025-09-30, 7d
    Relatórios, LGPD e Deploy      :done, b3, 2025-10-07, 7d

    section Fase 4: Frontend
    Web e Desktop                  :active, f1, 2025-10-14, 7d
    Mobile e Finalização           :f2, 2025-10-21, 7d

    section Fase 5: Documentação
    Trabalho ABNT (Paralelo)       :active, doc, 2025-10-07, 39d

    section Entrega
    Postagem UNIP                  :milestone, crit, entrega, 2025-11-17, 1d
```

---

## Marcos do Projeto

| Marco | Data | Status |
|-------|------|--------|
| M1 - Formação do Grupo | 30/08/2025 | ✅ Concluído |
| M2 - Requisitos e ER | 08/09/2025 | ✅ Concluído |
| M3 - Arquitetura e UML | 22/09/2025 | ✅ Concluído |
| M4 - API Funcional | 13/10/2025 | ✅ Concluído |
| M5 - Sistema Completo | 27/10/2025 | 🔄 Em Andamento |
| M6 - Documentação ABNT | 15/11/2025 | 🔄 Em Andamento |
| M7 - **ENTREGA UNIP** | **17/11/2025** | ⏳ Pendente |

---

## Como Visualizar

**Opção 1: GitHub** - Cole este arquivo no GitHub para visualização automática

**Opção 2: Mermaid Live** - https://mermaid.live/ (cole o código)

**Opção 3: VS Code** - Instale a extensão "Markdown Preview Mermaid Support"

---

**Legenda:**
- ✅ Verde (done): Concluído
- 🔵 Azul (active): Em andamento
- ⚪ Branco: Pendente
- 🔴 Vermelho (crit): Crítico
- 💎 Diamante (milestone): Marco

**Gerado em:** 21/10/2025
