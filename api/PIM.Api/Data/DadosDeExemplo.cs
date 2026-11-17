using Microsoft.EntityFrameworkCore;
using PIM.Api.Entities;
using PIM.Api.Services;
using PIM.Api.Utils;

namespace PIM.Api.Data
{
    /// <summary>
    /// Popula o banco com chamados de exemplo, para que dashboard, relatórios e listagens
    /// já apareçam preenchidos na primeira execução.
    /// </summary>
    /// <remarks>
    /// Só roda quando ainda não existe nenhum chamado e a configuração "Demo:DadosDeExemplo" está ligada.
    /// As datas são calculadas a partir do momento da execução, então os dados sempre parecem recentes.
    /// </remarks>
    public static class DadosDeExemplo
    {
        /// <summary>
        /// Descrição resumida de um chamado de exemplo.
        /// </summary>
        /// <param name="Dias">Há quantos dias o chamado foi aberto.</param>
        /// <param name="Criador">Primeiro nome do e-mail de quem abriu (ex.: "pedro").</param>
        /// <param name="Tecnico">Primeiro nome do e-mail do técnico, ou null se ninguém assumiu.</param>
        /// <param name="Horas">Horas entre a abertura e o encerramento (só para Resolvido e Fechado).</param>
        /// <param name="Nota">Nota da avaliação, de 1 a 5; zero deixa o chamado sem avaliação.</param>
        /// <param name="Conversa">Mensagens do chat, com "U:" para o usuário e "T:" para o técnico.</param>
        private sealed record Exemplo(
            int Dias,
            string Criador,
            string? Tecnico,
            string Categoria,
            string Prioridade,
            string Status,
            string Titulo,
            string Descricao,
            double Horas = 0,
            string? Solucao = null,
            int Nota = 0,
            string? Avaliacao = null,
            string[]? Conversa = null);

        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Chamados.AnyAsync())
            {
                return;
            }

            var agora = DateTime.Now;

            // Tudo ou nada: se algo falhar no meio, nenhum dado de exemplo fica gravado pela metade.
            await using var transacao = await context.Database.BeginTransactionAsync();

            await CriarUsuariosAsync(context, agora);

            var usuarios = new Dictionary<string, Usuario>();
            foreach (var usuario in await context.Usuarios.ToListAsync())
            {
                usuarios.TryAdd(usuario.Email.Split('.', '@')[0], usuario);
            }

            var categorias = await context.Categorias.ToDictionaryAsync(c => c.NomeCategoria, c => c.IdCategoria);
            var prioridades = await context.Prioridades.ToDictionaryAsync(p => p.NivelPrioridade, p => p.IdPrioridade);
            var status = await context.StatusChamados.ToDictionaryAsync(s => s.NomeStatus, s => s.IdStatus);

            var chamados = Exemplos
                .Select((exemplo, indice) => MontarChamado(exemplo, indice, agora, usuarios, categorias, prioridades, status))
                .OrderBy(c => c.DataAbertura)
                .ToList();

            // Um por vez e em ordem de data, para que os números dos chamados sigam a ordem em que foram abertos.
            foreach (var chamado in chamados)
            {
                context.Chamados.Add(chamado);
                await context.SaveChangesAsync();
            }

            await transacao.CommitAsync();
        }

        private static async Task CriarUsuariosAsync(ApplicationDbContext context, DateTime agora)
        {
            var perfilTecnico = await context.PerfisAcesso.FirstAsync(p => p.NomePerfil == "Técnico");
            var perfilUsuario = await context.PerfisAcesso.FirstAsync(p => p.NomePerfil == "Usuário");
            var setores = await context.Setores.ToDictionaryAsync(s => s.NomeSetor, s => s.IdSetor);

            var novos = new (string Nome, string Email, string Telefone, string Setor, PerfilAcesso Perfil)[]
            {
                ("Thiago Ribeiro", "thiago.ribeiro@empresa.com", "(11) 90987-6543", "TI", perfilTecnico),
                ("Fernanda Lima", "fernanda.lima@empresa.com", "(11) 99876-5432", "RH", perfilUsuario),
                ("Bruno Rocha", "bruno.rocha@empresa.com", "(11) 98765-1234", "Comercial", perfilUsuario),
                ("Juliana Martins", "juliana.martins@empresa.com", "(11) 97654-2345", "Financeiro", perfilUsuario),
                ("Rafael Mendes", "rafael.mendes@empresa.com", "(11) 96543-3456", "Administrativo", perfilUsuario)
            };

            var emailsExistentes = await context.Usuarios.Select(u => u.Email).ToListAsync();
            var senhaPadrao = PasswordHelper.HashPassword("senha123");

            foreach (var novo in novos.Where(n => !emailsExistentes.Contains(n.Email)))
            {
                context.Usuarios.Add(new Usuario
                {
                    NomeCompleto = novo.Nome,
                    Email = novo.Email,
                    Senha = senhaPadrao,
                    Telefone = novo.Telefone,
                    IdSetor = setores[novo.Setor],
                    IdPerfil = novo.Perfil.IdPerfil,
                    DataCadastro = agora.AddDays(-150),
                    PrimeiroAcesso = false,
                    Ativo = true
                });
            }

            await context.SaveChangesAsync();

            // Quem já passou pelo primeiro acesso aparece com um último login recente.
            var jaAcessaram = await context.Usuarios
                .Where(u => !u.PrimeiroAcesso && u.UltimoLogin == null)
                .OrderBy(u => u.IdUsuario)
                .ToListAsync();

            for (var i = 0; i < jaAcessaram.Count; i++)
            {
                jaAcessaram[i].UltimoLogin = agora.AddDays(-(i % 4)).AddHours(-(1 + (i * 3) % 7));
            }

            await context.SaveChangesAsync();
        }

        private static Chamado MontarChamado(
            Exemplo exemplo,
            int indice,
            DateTime agora,
            Dictionary<string, Usuario> usuarios,
            Dictionary<string, int> categorias,
            Dictionary<string, int> prioridades,
            Dictionary<string, int> status)
        {
            var criador = usuarios[exemplo.Criador];
            var tecnico = exemplo.Tecnico != null ? usuarios[exemplo.Tecnico] : null;

            // ---------- Linha do tempo ----------
            var abertura = CalcularAbertura(exemplo.Dias, indice, agora);
            var limite = agora.AddMinutes(-5);

            var assumiu = Antes(abertura.AddMinutes(20 + (indice * 7) % 70), limite);

            var encerrado = exemplo.Status is "Resolvido" or "Fechado";
            DateTime? fechamento = null;
            DateTime? resolvidoEm = null;

            if (encerrado)
            {
                var previsto = abertura.AddHours(exemplo.Horas);

                // No chamado fechado, a solução é registrada um pouco antes da confirmação do usuário.
                var solucao = exemplo.Status == "Fechado"
                    ? previsto.AddHours(-Math.Min(exemplo.Horas * 0.15, 3))
                    : previsto;

                resolvidoEm = Antes(Depois(NoExpediente(solucao), assumiu, 15), limite);
                fechamento = exemplo.Status == "Fechado"
                    ? Antes(Depois(NoExpediente(previsto), resolvidoEm.Value, 25), limite)
                    : resolvidoEm;
            }

            var chamado = new Chamado
            {
                Titulo = exemplo.Titulo,
                Descricao = exemplo.Descricao,
                DataAbertura = abertura,
                DataFechamento = fechamento,
                SolucaoFinal = encerrado ? exemplo.Solucao : null,
                IdUsuarioCriador = criador.IdUsuario,
                IdUsuarioTecnico = tecnico?.IdUsuario,
                IdCategoria = categorias[exemplo.Categoria],
                IdPrioridade = prioridades[exemplo.Prioridade],
                IdStatus = status[exemplo.Status],
                Historicos = [],
                Sugestoes = [],
                Comentarios = []
            };

            // ---------- Histórico de status ----------
            void Registrar(string? de, string para, DateTime quando, string observacao, int? idTecnico) =>
                chamado.Historicos.Add(new HistoricoChamado
                {
                    StatusAnterior = de,
                    StatusAtual = para,
                    DataHoraAlteracao = quando,
                    Observacao = observacao,
                    IdTecnico = idTecnico
                });

            Registrar(null, "Aberto", abertura, "Chamado criado", null);

            var espera = Antes(NoExpediente(assumiu.AddHours(2 + indice % 5)), limite);

            switch (exemplo.Status)
            {
                case "Aberto":
                    break;

                case "Cancelado":
                    Registrar("Aberto", "Cancelado", Antes(NoExpediente(abertura.AddHours(3)), limite), "Cancelado a pedido do usuário", null);
                    break;

                case "Em Análise":
                    Registrar("Aberto", "Em Análise", assumiu, "Chamado em triagem pela equipe de suporte", tecnico?.IdUsuario);
                    break;

                default:
                    Registrar("Aberto", "Em Andamento", assumiu, "Técnico assumiu o chamado", tecnico?.IdUsuario);

                    if (exemplo.Status == "Aguardando Usuário")
                    {
                        Registrar("Em Andamento", "Aguardando Usuário", espera, "Aguardando retorno do usuário com as informações solicitadas", tecnico?.IdUsuario);
                    }
                    else if (exemplo.Status == "Aguardando Fornecedor")
                    {
                        Registrar("Em Andamento", "Aguardando Fornecedor", espera, "Peça solicitada ao fornecedor, aguardando entrega", tecnico?.IdUsuario);
                    }
                    else if (encerrado)
                    {
                        Registrar("Em Andamento", "Resolvido", resolvidoEm!.Value, "Solução aplicada, aguardando confirmação do usuário", tecnico?.IdUsuario);

                        if (exemplo.Status == "Fechado")
                        {
                            Registrar("Resolvido", "Fechado", fechamento!.Value, "Chamado encerrado após confirmação do usuário", tecnico?.IdUsuario);
                        }
                    }

                    break;
            }

            // ---------- Sugestão da IA ----------
            if (tecnico != null && indice % 5 != 0)
            {
                chamado.Sugestoes.Add(new SugestaoIA
                {
                    TextoSugestao = ServicoIASimulada.MontarSugestao(exemplo.Titulo, exemplo.Descricao, exemplo.Categoria, exemplo.Prioridade),
                    DataHoraGeracao = Antes(assumiu.AddMinutes(4), limite),
                    FoiAceita = encerrado && indice % 3 != 0
                });
            }

            // ---------- Conversa no chat ----------
            if (exemplo.Conversa is { Length: > 0 } conversa && tecnico != null)
            {
                var inicio = assumiu;
                var fim = resolvidoEm ?? Antes(NoExpediente(assumiu.AddHours(6)), limite);
                if (fim <= inicio)
                {
                    fim = inicio.AddMinutes(conversa.Length + 1);
                }

                var intervalo = (fim - inicio) / (conversa.Length + 1);
                var anterior = inicio;

                for (var i = 0; i < conversa.Length; i++)
                {
                    var doTecnico = conversa[i].StartsWith("T:", StringComparison.Ordinal);
                    var ultima = i == conversa.Length - 1;

                    // Conversa que atravessa a noite continua na manhã seguinte, sempre em ordem.
                    var envio = Depois(Antes(NoExpediente(inicio + intervalo * (i + 1)), fim), anterior, 1);
                    anterior = envio;

                    chamado.Comentarios.Add(new ComentarioChamado
                    {
                        IdUsuario = doTecnico ? tecnico.IdUsuario : criador.IdUsuario,
                        Mensagem = conversa[i][2..].Trim(),
                        DataHoraEnvio = envio,
                        // Em chamado ainda aberto, a última mensagem fica como não lida.
                        Lido = encerrado || !ultima
                    });
                }
            }

            // ---------- Avaliação do usuário ----------
            if (exemplo.Status == "Fechado" && exemplo.Nota > 0)
            {
                chamado.Feedback = new Feedback
                {
                    Nota = exemplo.Nota,
                    Comentario = exemplo.Avaliacao,
                    DataHoraFeedback = Antes(NoExpediente(fechamento!.Value.AddHours(2 + indice % 9)), limite)
                };
            }

            return chamado;
        }

        /// <summary>
        /// Calcula a data de abertura em dia útil e em horário comercial (8h às 17h, no horário da máquina).
        /// </summary>
        private static DateTime CalcularAbertura(int diasAtras, int indice, DateTime agora)
        {
            var dia = agora.Date.AddDays(-diasAtras);

            if (dia.DayOfWeek == DayOfWeek.Saturday)
            {
                dia = dia.AddDays(-1);
            }
            else if (dia.DayOfWeek == DayOfWeek.Sunday)
            {
                dia = dia.AddDays(-2);
            }

            var abertura = dia.AddHours(8 + (indice * 7) % 9).AddMinutes((indice * 13) % 60);

            if (abertura <= agora.AddMinutes(-30))
            {
                return abertura;
            }

            // O horário escolhido ainda não chegou hoje: usa o trecho do expediente que já passou.
            var minutosDeExpediente = (int)(agora - agora.Date.AddHours(8)).TotalMinutes - 30;
            if (minutosDeExpediente >= 20)
            {
                return agora.Date.AddHours(8).AddMinutes((indice * 37) % minutosDeExpediente);
            }

            // Antes do expediente começar, o chamado fica no dia útil anterior.
            return CalcularAbertura(diasAtras + 1, indice, agora);
        }

        /// <summary>
        /// Traz o horário para o expediente (8h às 18h, em dia útil).
        /// O que cairia à noite ou no fim de semana vai para a manhã do próximo dia útil.
        /// </summary>
        private static DateTime NoExpediente(DateTime data)
        {
            if (data.Hour >= 18)
            {
                data = data.Date.AddDays(1).AddHours(8).AddMinutes(data.Minute + (data.Hour - 18) * 10);
            }
            else if (data.Hour < 8)
            {
                data = data.Date.AddHours(8).AddMinutes(data.Minute + data.Hour * 10);
            }

            while (data.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                data = data.AddDays(1);
            }

            return data;
        }

        private static DateTime Antes(DateTime data, DateTime limite) => data < limite ? data : limite;

        /// <summary>Garante que o evento venha depois do anterior, com a folga mínima em minutos.</summary>
        private static DateTime Depois(DateTime data, DateTime anterior, int folgaMinutos) =>
            data > anterior ? data : anterior.AddMinutes(folgaMinutos);

        private static readonly Exemplo[] Exemplos =
        [
            // ---------------- De 60 a 90 dias atrás ----------------
            new(88, "pedro", "joao", "Impressão", "Média", "Fechado",
                "Impressora do financeiro não imprime",
                "A impressora HP do setor financeiro não imprime desde ontem à tarde. Os documentos ficam parados na fila e nada sai.",
                Horas: 5, Solucao: "Fila de impressão limpa e serviço de spooler reiniciado. Impressão normalizada.",
                Nota: 5, Avaliacao: "Atendimento rápido, resolveu na hora.",
                Conversa:
                [
                    "U: Bom dia! A impressora continua sem imprimir.",
                    "T: Bom dia, Pedro. Vou acessar o seu computador agora para verificar a fila.",
                    "T: Limpei a fila e reiniciei o serviço de impressão. Pode testar?",
                    "U: Imprimiu! Obrigado."
                ]),

            new(85, "lucia", "maria", "Email", "Média", "Fechado",
                "Outlook não envia e-mails com anexo",
                "Quando tento enviar e-mail com anexo maior que 5 MB, o Outlook retorna erro e a mensagem fica parada na caixa de saída.",
                Horas: 20, Solucao: "Caixa postal estava cheia. Arquivamento configurado e limite de anexo explicado à usuária.",
                Nota: 4, Avaliacao: "Resolveu, mas demorou um pouco."),

            new(82, "roberto", "joao", "Hardware", "Alta", "Fechado",
                "Computador não liga na recepção",
                "O computador da recepção não liga. Aperto o botão e nenhuma luz acende. Já testei em outra tomada.",
                Horas: 26, Solucao: "Fonte de alimentação queimada. Peça substituída e equipamento testado.",
                Nota: 4, Avaliacao: "Bom atendimento."),

            new(80, "fernanda", "thiago", "Segurança", "Alta", "Fechado",
                "Senha do sistema de ponto bloqueada",
                "Errei a senha três vezes e o acesso ao sistema de ponto foi bloqueado. Preciso registrar a folha hoje.",
                Horas: 2, Solucao: "Conta desbloqueada e senha redefinida, com troca obrigatória no primeiro acesso.",
                Nota: 5, Avaliacao: "Muito rápido, obrigada!"),

            new(77, "bruno", "maria", "Rede", "Alta", "Fechado",
                "Wi-Fi caindo na sala de reuniões",
                "O Wi-Fi da sala de reuniões cai a cada 10 minutos. Atrapalha as videochamadas com clientes.",
                Horas: 30, Solucao: "Ponto de acesso com firmware desatualizado. Firmware atualizado e canal ajustado.",
                Nota: 3, Avaliacao: "Resolveu, mas ficamos dois dias com o problema."),

            new(74, "juliana", "joao", "Software", "Baixa", "Fechado",
                "Instalar Excel no notebook novo",
                "Recebi um notebook novo e ele veio sem o pacote Office. Preciso do Excel para as planilhas de fechamento.",
                Horas: 48, Solucao: "Office instalado e ativado com a licença do setor.",
                Nota: 5),

            new(71, "rafael", null, "Outros", "Baixa", "Cancelado",
                "Apoio da TI para evento interno",
                "Gostaria de apoio da TI para montar o projetor e o som no evento interno de sexta-feira."),

            new(69, "pedro", "thiago", "Software", "Média", "Fechado",
                "Planilha de fechamento travando",
                "A planilha de fechamento mensal trava toda vez que atualizo as tabelas dinâmicas. O computador fica sem responder.",
                Horas: 30, Solucao: "Memória insuficiente para o tamanho da planilha. Memória ampliada de 4 GB para 8 GB.",
                Nota: 4, Avaliacao: "Agora abre sem travar."),

            new(66, "lucia", "maria", "Impressão", "Baixa", "Fechado",
                "Trocar toner da impressora do comercial",
                "A impressora do comercial está avisando que o toner está no fim. Solicito a troca.",
                Horas: 70, Solucao: "Toner substituído.",
                Nota: 5),

            new(63, "roberto", "joao", "Rede", "Crítica", "Fechado",
                "Internet fora do ar em todo o administrativo",
                "Ninguém consegue acessar a internet no setor administrativo desde as 9h. Todos os computadores estão sem conexão.",
                Horas: 3, Solucao: "Switch do andar travado. Equipamento reiniciado e porta com defeito isolada.",
                Nota: 5, Avaliacao: "Resolveram muito rápido.",
                Conversa:
                [
                    "T: Roberto, identificamos que o switch do andar travou. Estamos reiniciando o equipamento.",
                    "U: Ok, estamos aguardando.",
                    "T: Internet restabelecida. Podem testar.",
                    "U: Voltou para todos. Obrigado!"
                ]),

            // ---------------- De 30 a 60 dias atrás ----------------
            new(58, "fernanda", "thiago", "Email", "Média", "Fechado",
                "Não recebo e-mails externos",
                "Desde segunda-feira não recebo e-mails de fora da empresa. Os internos chegam normalmente.",
                Horas: 22, Solucao: "Regra de bloqueio indevida no filtro de spam. Regra removida.",
                Nota: 4, Avaliacao: "Tudo certo agora."),

            new(56, "bruno", "maria", "Hardware", "Média", "Fechado",
                "Monitor piscando",
                "O monitor do meu computador pisca várias vezes por minuto. Fica difícil trabalhar.",
                Horas: 40, Solucao: "Cabo de vídeo com mau contato. Cabo substituído.",
                Nota: 5, Avaliacao: "Perfeito."),

            new(53, "juliana", "joao", "Segurança", "Crítica", "Fechado",
                "E-mail suspeito pedindo dados bancários",
                "Recebi um e-mail suspeito, em nome do banco, pedindo atualização de dados. Cliquei no link antes de perceber.",
                Horas: 3.5, Solucao: "Computador verificado com antivírus, sem infecção. Senhas trocadas e remetente bloqueado.",
                Nota: 5, Avaliacao: "Orientação muito clara."),

            new(51, "rafael", "thiago", "Software", "Alta", "Fechado",
                "Arquivos sumiram da pasta compartilhada",
                "Os arquivos da pasta compartilhada do administrativo sumiram. Eram os contratos deste ano.",
                Horas: 6, Solucao: "Pasta movida por engano. Arquivos restaurados do backup da noite anterior.",
                Nota: 5, Avaliacao: "Salvaram o nosso mês!",
                Conversa:
                [
                    "U: Preciso muito desses contratos para hoje.",
                    "T: Rafael, encontrei os arquivos no backup de ontem. Estou restaurando.",
                    "T: Arquivos de volta na pasta. Pode conferir?",
                    "U: Está tudo aqui. Muito obrigado!"
                ]),

            new(48, "pedro", "maria", "Impressão", "Média", "Fechado",
                "Scanner não digitaliza para a pasta de rede",
                "O scanner da impressora multifuncional não envia mais os documentos digitalizados para a pasta de rede.",
                Horas: 52, Solucao: "Senha da conta de serviço expirada. Credencial atualizada na impressora.",
                Nota: 3, Avaliacao: "Demorou mais do que eu esperava."),

            new(46, "lucia", "joao", "Hardware", "Média", "Fechado",
                "Microfone não funciona nas reuniões",
                "Meu microfone não funciona nas reuniões do Teams. As pessoas não me escutam.",
                Horas: 4, Solucao: "Dispositivo de entrada errado selecionado no Teams. Configuração corrigida.",
                Nota: 5),

            new(44, "roberto", null, "Software", "Baixa", "Cancelado",
                "Atualizar o leitor de PDF",
                "Gostaria de atualizar o leitor de PDF para a versão mais nova, quando for possível."),

            new(42, "fernanda", "thiago", "Software", "Crítica", "Fechado",
                "Sistema de folha de pagamento fora do ar",
                "O sistema de folha de pagamento está fora do ar para todo o RH. Hoje é o dia de fechamento da folha.",
                Horas: 2.5, Solucao: "Serviço do banco de dados parado após atualização. Serviço reiniciado e monitoramento criado.",
                Nota: 5, Avaliacao: "Excelente, voltou antes do prazo.",
                Conversa:
                [
                    "T: Fernanda, o serviço do banco de dados parou depois da atualização. Já estou reiniciando.",
                    "U: Obrigada, hoje é o fechamento da folha.",
                    "T: Sistema no ar. Pode acessar.",
                    "U: Funcionando!"
                ]),

            new(40, "bruno", "maria", "Rede", "Média", "Fechado",
                "VPN não conecta em casa",
                "Não consigo conectar a VPN quando trabalho de casa. Aparece erro de autenticação.",
                Horas: 28, Solucao: "Certificado da VPN vencido. Novo certificado instalado.",
                Nota: 4),

            new(37, "juliana", "joao", "Hardware", "Alta", "Fechado",
                "Notebook desliga sozinho",
                "O notebook desliga sozinho depois de uns 20 minutos de uso. Fica muito quente embaixo.",
                Horas: 20, Solucao: "Ventoinha obstruída por poeira. Limpeza interna e troca da pasta térmica.",
                Nota: 4, Avaliacao: "Funcionando bem."),

            new(35, "rafael", "thiago", "Email", "Baixa", "Fechado",
                "Criar assinatura padrão no e-mail",
                "Solicito a configuração da assinatura padrão da empresa no meu e-mail.",
                Horas: 90, Solucao: "Assinatura padrão configurada.",
                Nota: 5),

            new(33, "pedro", "maria", "Software", "Média", "Fechado",
                "Erro ao abrir o sistema financeiro",
                "O sistema financeiro mostra erro ao abrir: \"falha ao carregar módulo\". Reiniciei o computador e continua.",
                Horas: 18, Solucao: "Instalação corrompida. Sistema reinstalado e atualizado."),

            new(31, "lucia", "joao", "Rede", "Alta", "Fechado",
                "Sem internet no computador do comercial",
                "Meu computador está sem internet desde cedo. Os colegas ao lado estão navegando normalmente.",
                Horas: 3, Solucao: "Cabo de rede danificado. Cabo substituído.",
                Nota: 5, Avaliacao: "Rápido e educado."),

            // ---------------- Últimos 30 dias ----------------
            new(28, "fernanda", "thiago", "Hardware", "Baixa", "Fechado",
                "Solicitação de segundo monitor",
                "Solicito um segundo monitor para trabalhar com as planilhas de admissão.",
                Horas: 96, Solucao: "Monitor entregue e configurado."),

            new(26, "bruno", "maria", "Segurança", "Alta", "Fechado",
                "Esqueci a senha do CRM",
                "Esqueci minha senha do CRM e não consigo acessar a carteira de clientes.",
                Horas: 1.5, Solucao: "Senha redefinida.",
                Nota: 5, Avaliacao: "Na hora."),

            new(23, "juliana", "joao", "Impressão", "Média", "Fechado",
                "Papel atolado na impressora",
                "A impressora do financeiro está com papel atolado e mostra erro no painel.",
                Horas: 6, Solucao: "Papel removido e rolete de tração limpo."),

            new(21, "rafael", "thiago", "Software", "Média", "Fechado",
                "Computador muito lento",
                "Meu computador está muito lento há uma semana. Demora minutos para abrir qualquer programa.",
                Horas: 44, Solucao: "Disco quase cheio e muitos programas na inicialização. Limpeza feita e inicialização ajustada.",
                Nota: 2, Avaliacao: "Melhorou pouco, continua lento em alguns momentos.",
                Conversa:
                [
                    "T: Rafael, fiz a limpeza do disco e ajustei os programas da inicialização. Como ficou?",
                    "U: Melhorou um pouco, mas ainda trava às vezes."
                ]),

            new(18, "pedro", "maria", "Email", "Média", "Resolvido",
                "Caixa de e-mail cheia",
                "Minha caixa de e-mail está cheia e não recebo mais mensagens.",
                Horas: 10, Solucao: "Arquivamento automático configurado e mensagens antigas movidas."),

            new(16, "lucia", "joao", "Hardware", "Alta", "Resolvido",
                "Teclado do notebook com teclas falhando",
                "Várias teclas do teclado do notebook pararam de funcionar. Não consigo digitar as propostas.",
                Horas: 22, Solucao: "Teclado substituído."),

            new(14, "roberto", "thiago", "Rede", "Média", "Resolvido",
                "Internet lenta à tarde",
                "A internet fica muito lenta todo dia depois das 14h.",
                Horas: 36, Solucao: "Atualizações automáticas reprogramadas para fora do horário de expediente."),

            new(12, "fernanda", "maria", "Software", "Baixa", "Resolvido",
                "Instalar leitor de certificado digital",
                "Preciso instalar o programa do certificado digital para assinar documentos do RH.",
                Horas: 50, Solucao: "Programa instalado e certificado testado."),

            new(9, "bruno", "joao", "Hardware", "Alta", "Aguardando Fornecedor",
                "Tela do notebook quebrada",
                "A tela do meu notebook quebrou depois de uma queda. Aparece só uma mancha preta.",
                Conversa:
                [
                    "T: Bruno, a tela precisa ser trocada. Já pedi a peça ao fornecedor, com previsão de 5 dias úteis.",
                    "U: Entendi. Tem algum notebook reserva?",
                    "T: Sim, separei um para você. Pode retirar na TI."
                ]),

            new(7, "juliana", "thiago", "Software", "Alta", "Aguardando Usuário",
                "Erro ao gerar boleto no sistema financeiro",
                "O sistema financeiro mostra erro ao gerar boleto para alguns clientes. Preciso emitir as cobranças.",
                Conversa:
                [
                    "T: Juliana, pode me enviar o nome de um cliente com erro e a mensagem que aparece na tela?"
                ]),

            new(6, "rafael", "maria", "Email", "Média", "Aguardando Usuário",
                "E-mails caindo no spam do cliente",
                "Os e-mails que envio para um cliente específico estão caindo no spam dele.",
                Conversa:
                [
                    "T: Rafael, consegue me encaminhar um dos e-mails que caiu no spam do cliente?",
                    "U: Vou pedir para o cliente me reenviar e te mando."
                ]),

            new(5, "pedro", "joao", "Impressão", "Média", "Em Andamento",
                "Impressora imprimindo com manchas",
                "As páginas da impressora do financeiro saem com manchas pretas na lateral.",
                Conversa:
                [
                    "T: Pedro, vou passar aí depois do almoço para olhar o cilindro da impressora.",
                    "U: Combinado, obrigado."
                ]),

            new(4, "lucia", "thiago", "Rede", "Alta", "Em Andamento",
                "Wi-Fi não conecta no notebook",
                "Meu notebook não conecta no Wi-Fi da empresa. Pede a senha e diz que está incorreta.",
                Conversa:
                [
                    "U: Tentei a senha três vezes e não foi.",
                    "T: Lucia, a senha do Wi-Fi mudou na semana passada. Vou configurar no seu notebook."
                ]),

            new(3, "roberto", "maria", "Software", "Crítica", "Em Andamento",
                "Sistema de protocolo fora do ar",
                "O sistema de protocolo está fora do ar para todo o setor administrativo. Ninguém consegue registrar documentos.",
                Conversa:
                [
                    "T: Roberto, já achamos a causa: o servidor ficou sem espaço em disco. Estamos liberando.",
                    "U: Obrigado. O setor inteiro está parado."
                ]),

            new(3, "fernanda", "joao", "Hardware", "Média", "Em Andamento",
                "Câmera não funciona nas entrevistas online",
                "A câmera do meu computador não funciona nas entrevistas online. Aparece uma tela preta."),

            new(2, "bruno", "thiago", "Software", "Média", "Em Andamento",
                "CRM travando ao abrir relatórios",
                "O CRM trava sempre que abro o relatório de vendas do mês.",
                Conversa:
                [
                    "U: Trava sempre no relatório de vendas do mês.",
                    "T: Bruno, estou analisando o registro de erros do CRM. Já te retorno."
                ]),

            new(2, "juliana", "maria", "Segurança", "Alta", "Em Análise",
                "Acesso negado à pasta do financeiro",
                "Estou recebendo acesso negado ao abrir a pasta do financeiro na rede. Ontem funcionava."),

            new(1, "rafael", "joao", "Hardware", "Média", "Em Análise",
                "Mouse sem fio falhando",
                "O mouse sem fio falha e o cursor trava. Já troquei a pilha."),

            new(1, "pedro", "thiago", "Email", "Média", "Em Análise",
                "Outlook pedindo senha toda hora",
                "O Outlook pede a senha a cada poucos minutos, mesmo eu digitando a senha correta."),

            new(1, "lucia", null, "Impressão", "Média", "Aberto",
                "Impressora do comercial offline",
                "A impressora do comercial aparece como offline no meu computador e não imprime."),

            new(0, "roberto", null, "Rede", "Alta", "Aberto",
                "Sem internet na sala de arquivo",
                "O computador da sala de arquivo está sem internet. Preciso consultar processos com urgência."),

            new(0, "fernanda", null, "Software", "Baixa", "Aberto",
                "Atualizar o sistema de ponto",
                "Gostaria de solicitar a atualização do sistema de ponto para a versão nova, quando for possível."),

            new(0, "bruno", null, "Hardware", "Alta", "Aberto",
                "Notebook não liga",
                "Meu notebook não liga desde hoje cedo. Estava funcionando normalmente ontem."),

            new(0, "juliana", null, "Segurança", "Crítica", "Aberto",
                "Pop-up estranho pedindo para instalar antivírus",
                "Apareceu um pop-up estranho pedindo para instalar um antivírus e agora o navegador abre páginas sozinho.")
        ];
    }
}
