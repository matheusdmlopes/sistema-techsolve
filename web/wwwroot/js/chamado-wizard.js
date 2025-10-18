// Wizard de criação de chamado com IA
// Sistema TechSolve - PIM IV UNIP

// Variáveis globais para armazenar dados entre steps
let dadosProblema = {
    titulo: '',
    descricao: ''
};

let analiseIA = null;

// ========================================
// STEP 1: Descrever Problema
// ========================================

document.getElementById('btn-analisar').addEventListener('click', async function() {
    const titulo = document.getElementById('titulo').value.trim();
    const descricao = document.getElementById('descricao').value.trim();

    // Validação
    if (titulo.length < 5) {
        mostrarErro('titulo', 'O título deve ter pelo menos 5 caracteres');
        return;
    }

    if (descricao.length < 10) {
        mostrarErro('descricao', 'A descrição deve ter pelo menos 10 caracteres');
        return;
    }

    // Armazenar dados
    dadosProblema.titulo = titulo;
    dadosProblema.descricao = descricao;

    // Chamar IA
    await analisarComIA(titulo, descricao);
});

async function analisarComIA(titulo, descricao) {
    const btnAnalisar = document.getElementById('btn-analisar');
    const spinner = document.getElementById('spinner-analisar');

    try {
        // Mostrar loading
        btnAnalisar.disabled = true;
        spinner.classList.remove('d-none');
        btnAnalisar.innerHTML = '<i class="bi bi-robot"></i> Analisando... <span class="spinner-border spinner-border-sm" id="spinner-analisar"></span>';

        // Fazer requisição
        const response = await fetch('/Chamados/AnalisarProblema', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                titulo: titulo,
                descricao: descricao
            })
        });

        if (!response.ok) {
            const erro = await response.json();
            throw new Error(erro.erro || 'Erro ao analisar problema');
        }

        analiseIA = await response.json();

        // Ir para Step 2
        irParaStep(2);
        exibirSugestaoIA();

    } catch (error) {
        console.error('Erro ao analisar:', error);
        alert('Erro ao analisar problema com IA: ' + error.message + '\n\nVocê pode criar o chamado diretamente.');

        // Em caso de erro, permite criar chamado sem IA
        // irParaStep(3) diretamente se quiser pular a IA

    } finally {
        // Restaurar botão
        btnAnalisar.disabled = false;
        spinner.classList.add('d-none');
        btnAnalisar.innerHTML = '<i class="bi bi-robot"></i> Analisar com IA <span class="spinner-border spinner-border-sm d-none" id="spinner-analisar"></span>';
    }
}

// ========================================
// STEP 2: Sugestão da IA
// ========================================

function exibirSugestaoIA() {
    const container = document.getElementById('sugestao-container');

    if (!analiseIA) {
        container.innerHTML = '<div class="alert alert-danger">Erro ao carregar sugestão</div>';
        return;
    }

    const podeResolver = analiseIA.podeResolverSemTecnico;
    const iconeTipo = podeResolver
        ? '<i class="bi bi-check-circle-fill text-success" style="font-size: 3rem;"></i>'
        : '<i class="bi bi-tools text-warning" style="font-size: 3rem;"></i>';

    const mensagemTipo = podeResolver
        ? '<div class="alert alert-success"><strong><i class="bi bi-lightbulb"></i> Boa notícia!</strong> Você pode tentar resolver este problema seguindo as instruções abaixo:</div>'
        : '<div class="alert alert-info"><strong><i class="bi bi-info-circle"></i> Este problema pode precisar de suporte técnico,</strong> mas você pode tentar as sugestões abaixo primeiro:</div>';

    container.innerHTML = `
        <div class="text-center mb-4">
            ${iconeTipo}
        </div>

        ${mensagemTipo}

        <div class="card bg-light">
            <div class="card-body">
                <h5 class="card-title"><i class="bi bi-robot"></i> Sugestão de Solução</h5>
                <p class="card-text white-space-pre-line">${escapeHtml(analiseIA.sugestaoSolucao)}</p>
            </div>
        </div>

        <div class="row mt-4">
            <div class="col-md-6">
                <div class="card border-info">
                    <div class="card-body">
                        <h6 class="card-subtitle mb-2 text-muted">Categoria Identificada</h6>
                        <p class="card-text"><span class="badge bg-info">${escapeHtml(analiseIA.categoriaSugerida.nome)}</span></p>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card border-warning">
                    <div class="card-body">
                        <h6 class="card-subtitle mb-2 text-muted">Prioridade Avaliada</h6>
                        <p class="card-text"><span class="badge bg-warning text-dark">${escapeHtml(analiseIA.prioridadeSugerida.nivel)}</span></p>
                    </div>
                </div>
            </div>
        </div>
    `;
}

document.getElementById('btn-resolvido').addEventListener('click', function() {
    // Problema resolvido - não criar chamado
    if (confirm('Ótimo! Ficamos felizes que conseguiu resolver o problema.\n\nVocê será redirecionado para a página inicial.')) {
        window.location.href = '/Chamados/Index';
    }
});

document.getElementById('btn-nao-resolvido').addEventListener('click', function() {
    // Não resolveu - ir para Step 3 para criar chamado
    irParaStep(3);
    preencherFormularioComAnalise();
});

document.getElementById('btn-voltar-step1').addEventListener('click', function() {
    irParaStep(1);
});

// ========================================
// STEP 3: Confirmar e Criar Chamado
// ========================================

function preencherFormularioComAnalise() {
    // Preencher informações do problema
    document.getElementById('display-titulo').textContent = dadosProblema.titulo;
    document.getElementById('display-descricao').textContent = dadosProblema.descricao;
    document.getElementById('hidden-titulo').value = dadosProblema.titulo;
    document.getElementById('hidden-descricao').value = dadosProblema.descricao;

    if (analiseIA) {
        // Selecionar categoria e prioridade sugeridas pela IA
        document.getElementById('select-categoria').value = analiseIA.categoriaSugerida.idCategoria;
        document.getElementById('select-prioridade').value = analiseIA.prioridadeSugerida.idPrioridade;

        // Exibir sugestão resumida
        document.getElementById('resumo-sugestao').textContent = truncarTexto(analiseIA.sugestaoSolucao, 200);
    }
}

document.getElementById('btn-voltar-step2').addEventListener('click', function() {
    irParaStep(2);
});

// ========================================
// Funções Auxiliares
// ========================================

function irParaStep(numeroStep) {
    // Esconder todos os steps
    document.querySelectorAll('.wizard-content').forEach(el => {
        el.classList.add('d-none');
    });

    // Desativar todos os indicadores de step
    document.querySelectorAll('.wizard-step').forEach(el => {
        el.classList.remove('active');
    });

    // Mostrar step atual
    document.getElementById('step-' + numeroStep).classList.remove('d-none');
    document.getElementById('wizard-step-' + numeroStep).classList.add('active');

    // Scroll para o topo
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function mostrarErro(campoId, mensagem) {
    const campo = document.getElementById(campoId);
    campo.classList.add('is-invalid');

    const feedback = campo.nextElementSibling;
    if (feedback && feedback.classList.contains('invalid-feedback')) {
        feedback.textContent = mensagem;
    }

    campo.focus();

    // Remover erro ao digitar
    campo.addEventListener('input', function() {
        campo.classList.remove('is-invalid');
    }, { once: true });
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function truncarTexto(texto, maxLength) {
    if (texto.length <= maxLength) {
        return texto;
    }
    return texto.substring(0, maxLength) + '...';
}
