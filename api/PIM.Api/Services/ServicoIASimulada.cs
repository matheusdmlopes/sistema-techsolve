using System.Globalization;
using System.Text;

namespace PIM.Api.Services;

/// <summary>
/// Implementação do serviço de IA que funciona sem internet.
/// </summary>
/// <remarks>
/// Classifica o problema comparando o texto do chamado com uma base de conhecimento local
/// (tópicos de suporte com palavras-chave, análise e passos de solução).
/// É a implementação padrão, usada quando "IA:Modo" não está como "OpenAI".
/// </remarks>
public class ServicoIASimulada : IServicoIA
{
    private readonly int _atrasoMs;

    /// <param name="atrasoMs">Tempo médio de espera antes de responder, para simular o processamento. Zero desliga.</param>
    public ServicoIASimulada(int atrasoMs = 0)
    {
        _atrasoMs = Math.Max(0, atrasoMs);
    }

    public async Task<string> GerarSugestaoAsync(string titulo, string descricao, string? categoriaAtual, string? prioridadeAtual)
    {
        await SimularProcessamentoAsync();
        return MontarSugestao(titulo, descricao, categoriaAtual, prioridadeAtual);
    }

    public async Task<string> CategorizarAsync(string titulo, string descricao, IReadOnlyList<string> categorias)
    {
        await SimularProcessamentoAsync();
        return EscolherCategoria(titulo, descricao, categorias);
    }

    public async Task<string> PriorizarAsync(string titulo, string descricao, IReadOnlyList<string> prioridades)
    {
        await SimularProcessamentoAsync();
        return EscolherPrioridade(titulo, descricao, prioridades);
    }

    public async Task<string> AnalisarPreliminarAsync(string titulo, string descricao, IReadOnlyList<string> categorias, IReadOnlyList<string> prioridades)
    {
        await SimularProcessamentoAsync();
        return MontarAnalisePreliminar(titulo, descricao, categorias, prioridades);
    }

    private Task SimularProcessamentoAsync()
    {
        if (_atrasoMs == 0)
        {
            return Task.CompletedTask;
        }

        var minimo = (int)(_atrasoMs * 0.75);
        var maximo = (int)(_atrasoMs * 1.25) + 1;
        return Task.Delay(Random.Shared.Next(minimo, maximo));
    }

    // ------------------------------------------------------------------
    // Montagem das respostas (funções puras, usadas também pelo DataSeeder)
    // ------------------------------------------------------------------

    /// <summary>
    /// Monta a sugestão em texto livre exibida para o técnico dentro do chamado.
    /// </summary>
    public static string MontarSugestao(string titulo, string descricao, string? categoriaAtual, string? prioridadeAtual)
    {
        var (topico, pontos) = Classificar(titulo, descricao);
        var prioridade = CalcularPrioridade(topico, titulo, descricao);

        var texto = new StringBuilder();
        texto.AppendLine($"Análise: {topico.Analise}");
        texto.AppendLine();
        texto.AppendLine("Passos sugeridos:");
        for (var i = 0; i < topico.PassosTecnico.Length; i++)
        {
            texto.AppendLine($"{i + 1}. {topico.PassosTecnico[i]}");
        }

        texto.AppendLine();

        var ajustes = new List<string>();
        if (pontos > 0 && !MesmoTexto(categoriaAtual, topico.Categoria))
        {
            ajustes.Add($"a categoria para \"{topico.Categoria}\"");
        }

        if (!MesmoTexto(prioridadeAtual, prioridade))
        {
            ajustes.Add($"a prioridade para \"{prioridade}\"");
        }

        texto.Append(ajustes.Count > 0
            ? $"Recomendação: ajustar {string.Join(" e ", ajustes)}."
            : "Recomendação: a categoria e a prioridade atuais estão adequadas.");

        return texto.ToString();
    }

    /// <summary>
    /// Escolhe, entre as categorias cadastradas, a que melhor descreve o problema.
    /// </summary>
    public static string EscolherCategoria(string titulo, string descricao, IReadOnlyList<string> categorias)
    {
        var (topico, _) = Classificar(titulo, descricao);
        return ResolverNome(topico.Categoria, categorias, "Outros");
    }

    /// <summary>
    /// Escolhe, entre as prioridades cadastradas, a mais adequada ao impacto do problema.
    /// </summary>
    public static string EscolherPrioridade(string titulo, string descricao, IReadOnlyList<string> prioridades)
    {
        var (topico, _) = Classificar(titulo, descricao);
        return ResolverNome(CalcularPrioridade(topico, titulo, descricao), prioridades, "Média");
    }

    /// <summary>
    /// Monta a análise feita antes da abertura do chamado, no formato lido pelo IAPreliminarController.
    /// </summary>
    public static string MontarAnalisePreliminar(string titulo, string descricao, IReadOnlyList<string> categorias, IReadOnlyList<string> prioridades)
    {
        var (topico, _) = Classificar(titulo, descricao);
        var categoria = ResolverNome(topico.Categoria, categorias, "Outros");
        var prioridadeCalculada = CalcularPrioridade(topico, titulo, descricao);
        var prioridade = ResolverNome(prioridadeCalculada, prioridades, "Média");

        // Problema crítico afeta várias pessoas: não é algo para o usuário resolver sozinho.
        var usuarioResolve = topico.UsuarioResolve && prioridadeCalculada != "Crítica";

        var texto = new StringBuilder();
        texto.AppendLine($"CATEGORIA: {categoria}");
        texto.AppendLine($"PRIORIDADE: {prioridade}");
        texto.AppendLine($"PODE_RESOLVER: {(usuarioResolve ? "SIM" : "NÃO")}");
        texto.AppendLine("SOLUÇÃO:");
        texto.AppendLine(topico.Analise);

        if (usuarioResolve)
        {
            texto.AppendLine("Tente os passos abaixo antes de abrir o chamado:");
        }
        else
        {
            texto.AppendLine(topico.UsuarioResolve
                ? "Como o problema afeta várias pessoas, a equipe de TI precisa atuar com prioridade."
                : topico.MotivoTecnico);
            texto.AppendLine("Enquanto aguarda o atendimento:");
        }

        for (var i = 0; i < topico.PassosUsuario.Length; i++)
        {
            texto.AppendLine($"{i + 1}. {topico.PassosUsuario[i]}");
        }

        if (usuarioResolve)
        {
            texto.AppendLine("Se o problema continuar, abra o chamado e um técnico assume o atendimento.");
        }

        return texto.ToString();
    }

    // ------------------------------------------------------------------
    // Classificação
    // ------------------------------------------------------------------

    private static (Topico topico, int pontos) Classificar(string titulo, string descricao)
    {
        var textoTitulo = new TextoNormalizado(titulo);
        var textoDescricao = new TextoNormalizado(descricao);

        var melhor = TopicoGeral;
        var melhorPontos = 0;

        foreach (var topico in Topicos)
        {
            var pontos = 0;
            foreach (var termo in topico.Termos)
            {
                // Expressões com mais de uma palavra são mais específicas, por isso valem mais.
                var peso = termo.Contains(' ') ? 3 : 1;
                if (textoTitulo.Contem(termo))
                {
                    pontos += peso * 2;
                }

                if (textoDescricao.Contem(termo))
                {
                    pontos += peso;
                }
            }

            if (pontos > melhorPontos)
            {
                melhor = topico;
                melhorPontos = pontos;
            }
        }

        return (melhor, melhorPontos);
    }

    private static string CalcularPrioridade(Topico topico, string titulo, string descricao)
    {
        var texto = new TextoNormalizado($"{titulo} {descricao}");
        var nivel = Array.IndexOf(Niveis, topico.Prioridade);

        if (TermosImpactoGeral.Any(texto.Contem))
        {
            return "Crítica";
        }

        if (TermosSemPressa.Any(texto.Contem))
        {
            return Niveis[Math.Max(0, nivel - 1)];
        }

        if (TermosUrgencia.Any(texto.Contem))
        {
            return Niveis[Math.Max(nivel, Array.IndexOf(Niveis, "Alta"))];
        }

        if (topico.Prioridade == "Média" && TermosBaixoImpacto.Any(texto.Contem))
        {
            return "Baixa";
        }

        return topico.Prioridade;
    }

    /// <summary>
    /// Encontra, na lista cadastrada no sistema, o nome equivalente ao desejado.
    /// Se não existir, usa a reserva; se a reserva também não existir, usa o primeiro da lista.
    /// </summary>
    private static string ResolverNome(string desejado, IReadOnlyList<string> disponiveis, string reserva)
    {
        if (disponiveis.Count == 0)
        {
            return desejado;
        }

        return disponiveis.FirstOrDefault(d => MesmoTexto(d, desejado))
            ?? disponiveis.FirstOrDefault(d => MesmoTexto(d, reserva))
            ?? disponiveis[0];
    }

    private static bool MesmoTexto(string? a, string? b) =>
        a != null && b != null && TextoNormalizado.Normalizar(a) == TextoNormalizado.Normalizar(b);

    /// <summary>
    /// Texto em minúsculas, sem acentos e sem pontuação, pronto para comparação com os termos da base.
    /// </summary>
    private sealed class TextoNormalizado
    {
        private readonly string _texto;
        private readonly string[] _palavras;

        public TextoNormalizado(string? original)
        {
            var normalizado = Normalizar(original ?? string.Empty);
            _palavras = normalizado.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            _texto = $" {string.Join(' ', _palavras)} ";
        }

        /// <summary>
        /// Verifica se o termo aparece no texto.
        /// Termo com espaço é procurado como expressão; termo terminado em "*" casa com o início da palavra;
        /// os demais precisam ser a palavra inteira.
        /// </summary>
        public bool Contem(string termo)
        {
            if (termo.Contains(' '))
            {
                return _texto.Contains($" {termo}", StringComparison.Ordinal);
            }

            if (termo.EndsWith('*'))
            {
                var inicio = termo[..^1];
                return _palavras.Any(p => p.StartsWith(inicio, StringComparison.Ordinal));
            }

            return _palavras.Contains(termo);
        }

        public static string Normalizar(string original)
        {
            var decomposto = original.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var resultado = new StringBuilder(decomposto.Length);

            foreach (var caractere in decomposto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(caractere) == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                resultado.Append(char.IsLetterOrDigit(caractere) ? caractere : ' ');
            }

            // "wi-fi" e "e-mail" viram duas palavras ao remover o hífen; aqui voltam a ser uma só.
            return resultado.ToString()
                .Replace("wi fi", "wifi", StringComparison.Ordinal)
                .Replace("e mail", "email", StringComparison.Ordinal)
                .Replace("pop up", "popup", StringComparison.Ordinal);
        }
    }

    // ------------------------------------------------------------------
    // Base de conhecimento
    // ------------------------------------------------------------------

    private sealed record Topico(
        string Categoria,
        string Prioridade,
        bool UsuarioResolve,
        string[] Termos,
        string Analise,
        string MotivoTecnico,
        string[] PassosUsuario,
        string[] PassosTecnico);

    private static readonly string[] Niveis = ["Baixa", "Média", "Alta", "Crítica"];

    private static readonly string[] TermosImpactoGeral =
    [
        "todos os usuarios", "toda a empresa", "toda empresa", "empresa inteira", "todo o setor", "todo setor",
        "todos os computadores", "varios usuarios", "varios setores", "ninguem consegue", "parou tudo",
        "fora do ar", "producao parada"
    ];

    private static readonly string[] TermosSemPressa =
    [
        "sem pressa", "sem urgencia", "quando puder", "quando possivel", "quando for possivel"
    ];

    private static readonly string[] TermosUrgencia =
    [
        "urgente", "urgencia", "nao consigo trabalhar", "impedindo", "impede", "parado", "parada",
        "cliente esperando", "prazo", "o quanto antes", "imediat*"
    ];

    private static readonly string[] TermosBaixoImpacto =
    [
        "duvida*", "gostaria", "melhoria*", "sugestao"
    ];

    private static readonly Topico TopicoGeral = new(
        Categoria: "Outros",
        Prioridade: "Média",
        UsuarioResolve: false,
        Termos: [],
        Analise: "O relato não se encaixa em um problema conhecido da base de conhecimento e precisa da análise de um técnico.",
        MotivoTecnico: "Um técnico vai avaliar o caso e entrar em contato se precisar de mais detalhes.",
        PassosUsuario:
        [
            "Descreva o que você estava fazendo quando o problema apareceu.",
            "Informe se o problema acontece sempre ou só às vezes.",
            "Anexe uma foto ou captura da tela, se houver mensagem de erro."
        ],
        PassosTecnico:
        [
            "Entrar em contato com o usuário para detalhar o problema.",
            "Reproduzir a falha no equipamento do usuário.",
            "Consultar chamados anteriores parecidos.",
            "Reclassificar a categoria e a prioridade após o diagnóstico."
        ]);

    // A ordem importa em caso de empate: vence o tópico que aparece primeiro.
    private static readonly Topico[] Topicos =
    [
        new(
            Categoria: "Segurança",
            Prioridade: "Crítica",
            UsuarioResolve: false,
            Termos:
            [
                "virus", "malware", "phishing", "ransomware", "antivirus", "invadid*", "invas*", "suspeit*",
                "golpe*", "hacke*", "vazamento", "popup*", "propaganda*", "link estranho", "cliquei no link",
                "email suspeito", "email estranho", "arquivos criptografados"
            ],
            Analise: "O relato indica um possível incidente de segurança, que pode se espalhar para outros computadores da rede.",
            MotivoTecnico: "Incidentes de segurança exigem análise e contenção pela equipe de TI.",
            PassosUsuario:
            [
                "Desconecte o computador da rede (cabo ou Wi-Fi) e não o desligue.",
                "Não clique em links, não abra anexos e não informe senhas.",
                "Avise a TI imediatamente e informe o que foi aberto ou clicado."
            ],
            PassosTecnico:
            [
                "Isolar o equipamento da rede para conter a ameaça.",
                "Executar verificação completa com o antivírus atualizado.",
                "Trocar as senhas do usuário e revisar os acessos recentes.",
                "Registrar o incidente e orientar o usuário sobre prevenção."
            ]),

        new(
            Categoria: "Software",
            Prioridade: "Crítica",
            UsuarioResolve: false,
            Termos:
            [
                "erp", "servidor*", "indisponivel", "fora do ar", "sistema fora", "sistema caiu", "sistema parado",
                "sistema nao abre", "banco de dados", "erro 500", "nao carrega"
            ],
            Analise: "O relato indica indisponibilidade de um sistema usado por vários setores, com impacto direto na operação.",
            MotivoTecnico: "A falha está no servidor ou na aplicação, e só a equipe de TI tem acesso para corrigir.",
            PassosUsuario:
            [
                "Confirme com um colega se o sistema também está fora do ar para ele.",
                "Anote o horário em que o problema começou e a mensagem de erro exibida.",
                "Evite repetir a operação várias vezes até a TI liberar o uso."
            ],
            PassosTecnico:
            [
                "Confirmar a abrangência da falha e comunicar os setores afetados.",
                "Verificar o estado do servidor, dos serviços da aplicação e do banco de dados.",
                "Analisar os registros de erro e reiniciar o serviço com falha.",
                "Registrar a causa e a correção aplicada para evitar reincidência."
            ]),

        new(
            Categoria: "Impressão",
            Prioridade: "Média",
            UsuarioResolve: true,
            Termos:
            [
                "impress*", "imprim*", "toner", "cartucho*", "scanner*", "digitaliz*", "escane*", "atolad*",
                "papel atolado", "fila de impressao"
            ],
            Analise: "O relato indica falha de comunicação ou de suprimento na impressora, um problema comum e normalmente simples de resolver.",
            MotivoTecnico: "",
            PassosUsuario:
            [
                "Confira se a impressora está ligada, sem luz de erro, com papel e com toner ou tinta.",
                "Abra a fila de impressão, cancele os documentos parados e envie o arquivo de novo.",
                "Desligue a impressora, aguarde 30 segundos e ligue novamente.",
                "Verifique se a impressora certa está selecionada na hora de imprimir."
            ],
            PassosTecnico:
            [
                "Limpar a fila de impressão e reiniciar o serviço Spooler no computador do usuário.",
                "Testar a comunicação com a impressora (ping no IP e página de teste).",
                "Reinstalar ou atualizar o driver da impressora.",
                "Se houver falha física ou falta de suprimento, acionar o fornecedor do equipamento."
            ]),

        new(
            Categoria: "Segurança",
            Prioridade: "Alta",
            UsuarioResolve: false,
            Termos:
            [
                "senha*", "login", "logar", "bloquead*", "desbloque*", "esqueci*", "redefin*", "expir*", "permiss*",
                "acesso negado", "nao consigo entrar", "nao consigo acessar", "usuario bloqueado"
            ],
            Analise: "O relato indica problema de autenticação: senha esquecida, expirada ou conta bloqueada por tentativas incorretas.",
            MotivoTecnico: "A redefinição de senha e o desbloqueio de conta só podem ser feitos pela equipe de TI, por segurança.",
            PassosUsuario:
            [
                "Confira se o Caps Lock está desligado e se o e-mail foi digitado corretamente.",
                "Não tente várias vezes seguidas, para não bloquear a conta.",
                "Informe no chamado em qual sistema o acesso falhou e a mensagem exibida."
            ],
            PassosTecnico:
            [
                "Confirmar a identidade do usuário antes de qualquer alteração.",
                "Verificar se a conta está bloqueada ou com a senha expirada.",
                "Redefinir a senha e marcar a troca obrigatória no primeiro acesso.",
                "Orientar o usuário sobre a política de senhas da empresa."
            ]),

        new(
            Categoria: "Rede",
            Prioridade: "Alta",
            UsuarioResolve: true,
            Termos:
            [
                "internet", "wifi", "rede", "redes", "conexao", "conect*", "desconect*", "roteador*", "vpn", "ip",
                "site", "sites", "cabo de rede", "sem sinal", "sem conexao", "sem internet", "internet lenta",
                "caiu a internet", "nenhum site", "site nao abre", "sites nao abrem"
            ],
            Analise: "O relato indica falha de conectividade, que pode estar no cabo, no Wi-Fi ou no equipamento de rede do setor.",
            MotivoTecnico: "",
            PassosUsuario:
            [
                "Verifique se o cabo de rede está bem encaixado ou se o Wi-Fi está ligado e conectado à rede da empresa.",
                "Desative e ative a conexão de rede, ou reinicie o computador.",
                "Tente abrir outro site ou sistema para saber se a falha é geral.",
                "Pergunte a um colega do mesmo setor se a internet dele está funcionando."
            ],
            PassosTecnico:
            [
                "Verificar o link de internet e o estado do switch e do roteador do setor.",
                "Testar a conexão do computador (ipconfig, ping no gateway e em um endereço externo).",
                "Renovar o endereço IP e limpar o cache de DNS.",
                "Se vários usuários forem afetados, tratar como incidente e acionar o provedor."
            ]),

        new(
            Categoria: "Email",
            Prioridade: "Média",
            UsuarioResolve: true,
            Termos:
            [
                "email*", "outlook", "webmail", "spam", "anexo*", "caixa de entrada", "caixa postal", "caixa cheia",
                "nao envia", "nao recebe", "nao recebo"
            ],
            Analise: "O relato indica falha no envio ou no recebimento de e-mails, geralmente ligada a caixa cheia, conexão ou configuração da conta.",
            MotivoTecnico: "",
            PassosUsuario:
            [
                "Feche e abra o programa de e-mail e confira se há internet.",
                "Veja se a caixa de entrada está cheia e apague mensagens antigas ou com anexos grandes.",
                "Confira a pasta de spam e a caixa de saída.",
                "Teste o acesso pelo webmail para saber se o problema é só no programa."
            ],
            PassosTecnico:
            [
                "Verificar a cota da caixa postal e o estado da conta no servidor de e-mail.",
                "Testar envio e recebimento pelo webmail.",
                "Recriar o perfil de e-mail no computador, se o problema for local.",
                "Checar filas e regras de bloqueio no servidor, se o problema for geral."
            ]),

        new(
            Categoria: "Software",
            Prioridade: "Alta",
            UsuarioResolve: false,
            Termos:
            [
                "backup*", "recuper*", "restaur*", "corrompid*", "apaguei", "exclui", "excluido*", "perdi", "sumiu",
                "sumiram", "arquivo apagado", "arquivos apagados", "pasta compartilhada", "pasta de rede"
            ],
            Analise: "O relato indica perda ou indisponibilidade de arquivos, que pode ser revertida pela cópia de segurança se for tratada rápido.",
            MotivoTecnico: "A recuperação depende do acesso ao servidor de arquivos e às cópias de segurança.",
            PassosUsuario:
            [
                "Procure o arquivo na Lixeira e use a busca do computador pelo nome.",
                "Evite salvar arquivos novos no mesmo local até a TI verificar.",
                "Informe o nome do arquivo, a pasta onde estava e quando foi visto pela última vez."
            ],
            PassosTecnico:
            [
                "Verificar a Lixeira local e as versões anteriores da pasta.",
                "Localizar o arquivo na cópia de segurança mais recente.",
                "Restaurar o arquivo para o local original e validar com o usuário.",
                "Revisar as permissões da pasta compartilhada."
            ]),

        new(
            Categoria: "Software",
            Prioridade: "Média",
            UsuarioResolve: true,
            Termos:
            [
                "lent*", "trav*", "congel*", "demora*", "devagar", "computador lento", "muito lento", "nao responde"
            ],
            Analise: "O relato indica lentidão ou travamento, normalmente causado por muitos programas abertos, pouca memória livre ou disco cheio.",
            MotivoTecnico: "",
            PassosUsuario:
            [
                "Feche os programas e as abas do navegador que não estão em uso.",
                "Reinicie o computador, principalmente se ele está ligado há vários dias.",
                "Verifique se há espaço livre no disco e apague arquivos de que não precisa mais.",
                "Observe se a lentidão acontece em tudo ou só em um programa específico."
            ],
            PassosTecnico:
            [
                "Verificar o uso de CPU, memória e disco no Gerenciador de Tarefas.",
                "Remover programas desnecessários da inicialização e limpar arquivos temporários.",
                "Executar verificação de vírus e de integridade do disco.",
                "Avaliar aumento de memória ou troca por SSD, se o equipamento for antigo."
            ]),

        new(
            Categoria: "Hardware",
            Prioridade: "Média",
            UsuarioResolve: true,
            Termos:
            [
                "camera", "webcam", "microfone*", "audio", "som", "teams", "zoom", "meet", "fone*", "headset",
                "videoconferencia", "videochamada", "caixa de som", "reuniao online"
            ],
            Analise: "O relato indica falha de áudio ou de vídeo, geralmente ligada à seleção do dispositivo ou à permissão do aplicativo.",
            MotivoTecnico: "",
            PassosUsuario:
            [
                "Confira se o fone, o microfone ou a câmera estão bem conectados e sem o botão de mudo ativado.",
                "Nas configurações da reunião, selecione o microfone e a câmera corretos.",
                "Feche outros programas que possam estar usando a câmera ou o microfone.",
                "Reinicie o aplicativo de reunião e teste novamente."
            ],
            PassosTecnico:
            [
                "Verificar os dispositivos de entrada e de saída padrão do sistema.",
                "Conferir as permissões de câmera e de microfone do aplicativo.",
                "Atualizar ou reinstalar os drivers de áudio e de vídeo.",
                "Testar com outro periférico para descartar defeito físico."
            ]),

        new(
            Categoria: "Hardware",
            Prioridade: "Alta",
            UsuarioResolve: false,
            Termos:
            [
                "monitor*", "teclado*", "mouse", "fonte", "barulho*", "superaquec*", "esquent*", "bateria",
                "carregador*", "hd", "ssd", "queim*", "quebr*", "notebook*", "nao liga", "nao ligam", "tela preta",
                "tela azul", "desliga sozinho", "reinicia sozinho", "cheiro de queimado", "pilha*", "usb", "pendrive",
                "pen drive", "hdmi", "projetor*", "periferico*", "mouse sem fio", "teclado sem fio", "nao reconhece"
            ],
            Analise: "O relato indica defeito físico no equipamento, que precisa de avaliação presencial.",
            MotivoTecnico: "Abrir o equipamento ou trocar peças só pode ser feito por um técnico, para evitar danos e perda de garantia.",
            PassosUsuario:
            [
                "Confira se os cabos de energia e de vídeo estão bem encaixados e se a tomada funciona.",
                "Não abra o equipamento nem insista em ligá-lo se houver cheiro de queimado ou barulho estranho.",
                "Informe no chamado o número do patrimônio e o que aconteceu antes do defeito."
            ],
            PassosTecnico:
            [
                "Testar cabos, fonte e tomada antes de abrir o equipamento.",
                "Isolar o defeito testando com periféricos e peças reservas.",
                "Fazer cópia dos dados do usuário antes de qualquer troca de disco.",
                "Substituir a peça com defeito ou acionar a garantia do fabricante."
            ]),

        new(
            Categoria: "Software",
            Prioridade: "Baixa",
            UsuarioResolve: false,
            Termos:
            [
                "instal*", "desinstal*", "licenca*", "atualiz*", "versao", "ativacao", "office", "excel", "word",
                "powerpoint", "planilha*", "programa*", "aplicativo*", "software*", "erro ao abrir", "nao abre"
            ],
            Analise: "O relato indica necessidade de instalação, atualização ou correção de um programa.",
            MotivoTecnico: "A instalação de programas exige permissão de administrador e verificação da licença.",
            PassosUsuario:
            [
                "Informe o nome e a versão do programa e para que ele será usado.",
                "Se houver mensagem de erro, copie o texto ou anexe uma foto da tela.",
                "Salve seu trabalho e reinicie o computador para descartar uma falha passageira."
            ],
            PassosTecnico:
            [
                "Confirmar se há licença disponível e se o programa é homologado pela empresa.",
                "Instalar ou atualizar o programa com a conta de administrador.",
                "Em caso de erro, reparar a instalação ou reinstalar o programa.",
                "Testar junto com o usuário antes de encerrar o chamado."
            ]),

        new(
            Categoria: "Hardware",
            Prioridade: "Baixa",
            UsuarioResolve: false,
            Termos:
            [
                "solicit*", "substitu*", "adquirir", "compra*", "emprest*", "trocar", "troca", "novo equipamento",
                "novo computador", "novo notebook", "novo monitor", "novo colaborador", "novo funcionario"
            ],
            Analise: "O relato é uma solicitação de equipamento ou de troca, que depende de estoque e de aprovação.",
            MotivoTecnico: "A entrega de equipamentos é controlada pela TI, que confere o estoque e registra o patrimônio.",
            PassosUsuario:
            [
                "Informe o motivo da solicitação e para quem é o equipamento.",
                "Indique a data em que o equipamento precisa estar disponível.",
                "Peça ao gestor do setor para registrar a aprovação no chamado."
            ],
            PassosTecnico:
            [
                "Verificar a disponibilidade do item no estoque.",
                "Confirmar a aprovação do gestor do setor.",
                "Preparar o equipamento com os programas padrão da empresa.",
                "Registrar a entrega e atualizar o controle de patrimônio."
            ])
    ];
}
