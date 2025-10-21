// Sistema de Chat em Tempo Real com SignalR
// TechSolve - PIM IV UNIP

let connection = null;
let idChamado = null;
let idUsuario = null;
let nomeUsuario = null;

// Inicializar chat
function inicializarChat(chamadoId, usuarioId, usuarioNome, apiBaseUrl) {
    idChamado = chamadoId;
    idUsuario = usuarioId;
    nomeUsuario = usuarioNome;

    console.log(`Inicializando chat: Chamado=${chamadoId}, Usuario=${usuarioId}, API=${apiBaseUrl}`);

    // Criar conexão SignalR
    connection = new signalR.HubConnectionBuilder()
        .withUrl(apiBaseUrl + "/chatHub")
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

    // Configurar eventos
    configurarEventosSignalR();

    // Conectar
    connection.start()
        .then(() => {
            console.log("✅ Conectado ao SignalR Hub");
            document.getElementById(`badge-online-${idChamado}`).innerHTML = '<i class="bi bi-circle-fill text-success" style="font-size: 0.5rem;"></i> Online';

            // Entrar na sala do chamado
            return connection.invoke("EntrarNoChamado", idChamado);
        })
        .then(() => {
            console.log(`✅ Entrou na sala do chamado ${idChamado}`);
            // Carregar mensagens existentes
            carregarMensagens();
        })
        .catch(err => {
            console.error("❌ Erro ao conectar SignalR:", err);
            document.getElementById(`badge-online-${idChamado}`).innerHTML = '<i class="bi bi-circle-fill text-danger" style="font-size: 0.5rem;"></i> Offline';
            mostrarErro("Erro ao conectar ao chat. Tentando fallback REST...");
            // Carregar mensagens via REST mesmo sem SignalR
            carregarMensagens();
        });

    // Configurar botão enviar
    const btnEnviar = document.getElementById(`btn-enviar-mensagem-${idChamado}`);
    if (btnEnviar) {
        btnEnviar.addEventListener("click", enviarMensagem);
    }

    // Enviar com Enter
    const inputMensagem = document.getElementById(`input-mensagem-${idChamado}`);
    if (inputMensagem) {
        inputMensagem.addEventListener("keypress", function(e) {
            if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                enviarMensagem();
            }
        });

        // Indicador de digitação
        let digitandoTimeout;
        inputMensagem.addEventListener("input", function() {
            if (connection && connection.state === signalR.HubConnectionState.Connected) {
                connection.invoke("UsuarioDigitando", idChamado, nomeUsuario);

                clearTimeout(digitandoTimeout);
                digitandoTimeout = setTimeout(() => {
                    connection.invoke("UsuarioParouDeDigitar", idChamado);
                }, 1000);
            }
        });
    }
}

// Configurar eventos do SignalR
function configurarEventosSignalR() {
    // Receber nova mensagem
    connection.on("ReceberMensagem", function(comentario) {
        console.log("📩 Nova mensagem recebida:", comentario);
        adicionarMensagemNaUI(comentario);

        // Marcar como lida se não for minha
        if (comentario.idUsuario !== idUsuario) {
            marcarComoLida();
        }
    });

    // Alguém está digitando
    connection.on("UsuarioDigitando", function(nome) {
        const indicador = document.getElementById(`indicador-digitando-${idChamado}`);
        if (indicador) {
            indicador.querySelector('span:last-child').textContent = nome + " está digitando...";
            indicador.classList.add("show");
        }
    });

    // Parou de digitar
    connection.on("UsuarioParouDeDigitar", function() {
        const indicador = document.getElementById(`indicador-digitando-${idChamado}`);
        if (indicador) {
            indicador.classList.remove("show");
        }
    });

    // Reconexão
    connection.onreconnecting(() => {
        console.log("🔄 Reconectando...");
        document.getElementById(`badge-online-${idChamado}`).innerHTML = '<i class="bi bi-circle-fill text-warning" style="font-size: 0.5rem;"></i> Reconectando...';
        mostrarInfo("Reconectando ao chat...");
    });

    connection.onreconnected(() => {
        console.log("✅ Reconectado!");
        document.getElementById(`badge-online-${idChamado}`).innerHTML = '<i class="bi bi-circle-fill text-success" style="font-size: 0.5rem;"></i> Online';
        connection.invoke("EntrarNoChamado", idChamado);
        mostrarSucesso("Reconectado ao chat!");
    });
}

// Carregar mensagens existentes
async function carregarMensagens() {
    try {
        console.log(`📥 Carregando mensagens do chamado ${idChamado}...`);
        const response = await fetch(`/Chamados/ObterComentarios?idChamado=${idChamado}`);
        if (response.ok) {
            const comentarios = await response.json();
            console.log(`✅ ${comentarios.length} mensagens carregadas`);

            const container = document.getElementById(`chat-mensagens-${idChamado}`);
            if (!container) {
                console.error("❌ Container de mensagens não encontrado!");
                return;
            }

            container.innerHTML = ""; // Limpar

            if (comentarios.length === 0) {
                container.innerHTML = `
                    <div class="chat-empty-state">
                        <i class="bi bi-chat-left-text"></i>
                        <p>Nenhuma mensagem ainda.<br>Seja o primeiro a enviar!</p>
                    </div>
                `;
            } else {
                comentarios.forEach(c => adicionarMensagemNaUI(c));
            }

            // Scroll para o final
            scrollParaUltimaMensagem();
        } else {
            console.error("❌ Erro ao carregar mensagens:", response.status);
        }
    } catch (error) {
        console.error("❌ Erro ao carregar mensagens:", error);
    }
}

// Enviar mensagem
async function enviarMensagem() {
    const input = document.getElementById(`input-mensagem-${idChamado}`);
    if (!input) {
        console.error("❌ Input de mensagem não encontrado!");
        return;
    }

    const mensagem = input.value.trim();

    if (!mensagem) {
        console.warn("⚠️ Mensagem vazia, ignorando envio");
        return;
    }

    const btnEnviar = document.getElementById(`btn-enviar-mensagem-${idChamado}`);
    if (btnEnviar) btnEnviar.disabled = true;

    if (connection && connection.state === signalR.HubConnectionState.Connected) {
        try {
            console.log(`📤 Enviando mensagem via SignalR...`);
            // Enviar via SignalR
            await connection.invoke("EnviarMensagem", idChamado, idUsuario, mensagem);
            console.log("✅ Mensagem enviada com sucesso!");

            // Limpar input
            input.value = "";

            // Parar indicador de digitação
            connection.invoke("UsuarioParouDeDigitar", idChamado);
        } catch (error) {
            console.error("❌ Erro ao enviar via SignalR:", error);
            console.log("🔄 Tentando fallback REST...");
            // Fallback REST se SignalR falhar
            await enviarMensagemREST(mensagem);
            input.value = "";
        }
    } else {
        console.log("⚠️ SignalR não conectado, usando fallback REST");
        // Fallback: enviar via REST API se SignalR não conectado
        await enviarMensagemREST(mensagem);
        input.value = "";
    }

    if (btnEnviar) btnEnviar.disabled = false;
    input.focus();
}

// Fallback REST API
async function enviarMensagemREST(mensagem) {
    try {
        const response = await fetch(`/Chamados/EnviarComentario?idChamado=${idChamado}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ mensagem: mensagem })
        });

        if (response.ok) {
            const comentario = await response.json();
            adicionarMensagemNaUI(comentario);
        } else {
            mostrarErro("Erro ao enviar mensagem");
        }
    } catch (error) {
        console.error("Erro REST:", error);
        mostrarErro("Erro ao enviar mensagem");
    }
}

// Adicionar mensagem na UI
function adicionarMensagemNaUI(comentario) {
    const container = document.getElementById(`chat-mensagens-${idChamado}`);
    if (!container) {
        console.error("❌ Container de mensagens não encontrado!");
        return;
    }

    // Limpar estado vazio se existir
    const emptyState = container.querySelector('.chat-empty-state');
    if (emptyState) {
        emptyState.remove();
    }

    const ehMinha = comentario.idUsuario === idUsuario;
    const grupoClass = ehMinha ? "minha-mensagem" : "outra-mensagem";
    const balaoClass = ehMinha ? "minha-mensagem" : (comentario.ehTecnico ? "mensagem-tecnico" : "outra-mensagem");
    const nomeExibir = ehMinha ? "Você" : escapeHtml(comentario.nomeUsuario);

    const data = new Date(comentario.dataHoraEnvio);
    const horaFormatada = data.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });

    const mensagemHtml = `
        <div class="chat-mensagem-grupo ${grupoClass}">
            <div class="chat-balao ${balaoClass}">
                <span class="chat-remetente">${nomeExibir}</span>
                <div class="chat-texto">${escapeHtml(comentario.mensagem)}</div>
                <span class="chat-timestamp">${horaFormatada}</span>
            </div>
        </div>
    `;

    container.insertAdjacentHTML('beforeend', mensagemHtml);
    scrollParaUltimaMensagem();
}

// Marcar como lida
async function marcarComoLida() {
    try {
        await fetch(`/Chamados/MarcarComentariosLidos?idChamado=${idChamado}`, {
            method: 'PUT'
        });
    } catch (error) {
        console.error("Erro ao marcar como lida:", error);
    }
}

// Scroll automático
function scrollParaUltimaMensagem() {
    const container = document.getElementById(`chat-mensagens-${idChamado}`);
    if (container) {
        container.scrollTop = container.scrollHeight;
    }
}

// Escape HTML para segurança
function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Notificações
function mostrarErro(mensagem) {
    console.error(mensagem);
    // Você pode adicionar toast/alert aqui
}

function mostrarSucesso(mensagem) {
    console.log(mensagem);
}

function mostrarInfo(mensagem) {
    console.info(mensagem);
}

// Desconectar ao sair da página
window.addEventListener('beforeunload', () => {
    if (connection) {
        connection.invoke("SairDoChamado", idChamado);
        connection.stop();
    }
});
