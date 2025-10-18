using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PIM.Api.Data;
using PIM.Api.Entities;

namespace PIM.Api.Hubs;

/// <summary>
/// Hub SignalR para chat em tempo real entre técnicos e usuários nos chamados
/// </summary>
public class ChatHub : Hub
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ApplicationDbContext context, ILogger<ChatHub> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Entra em uma "sala" de chat de um chamado específico
    /// </summary>
    public async Task EntrarNoChamado(int idChamado)
    {
        var groupName = $"Chamado_{idChamado}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        _logger.LogInformation("Usuário {ConnectionId} entrou no chamado {IdChamado}",
            Context.ConnectionId, idChamado);
    }

    /// <summary>
    /// Sai da "sala" de chat de um chamado
    /// </summary>
    public async Task SairDoChamado(int idChamado)
    {
        var groupName = $"Chamado_{idChamado}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        _logger.LogInformation("Usuário {ConnectionId} saiu do chamado {IdChamado}",
            Context.ConnectionId, idChamado);
    }

    /// <summary>
    /// Envia uma mensagem para todos os usuários conectados ao chamado
    /// </summary>
    public async Task EnviarMensagem(int idChamado, int idUsuario, string mensagem)
    {
        try
        {
            // Validar se chamado existe
            var chamadoExiste = await _context.Chamados.AnyAsync(c => c.IdChamado == idChamado);
            if (!chamadoExiste)
            {
                _logger.LogWarning("Tentativa de enviar mensagem para chamado inexistente: {IdChamado}", idChamado);
                return;
            }

            // Buscar informações do usuário
            var usuario = await _context.Usuarios
                .Include(u => u.PerfilAcesso)
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

            if (usuario == null)
            {
                _logger.LogWarning("Usuário não encontrado: {IdUsuario}", idUsuario);
                return;
            }

            // Salvar comentário no banco
            var comentario = new ComentarioChamado
            {
                IdChamado = idChamado,
                IdUsuario = idUsuario,
                Mensagem = mensagem,
                DataHoraEnvio = DateTime.UtcNow,
                Lido = false
            };

            _context.ComentariosChamados.Add(comentario);
            await _context.SaveChangesAsync();

            // Preparar dados para broadcast
            var comentarioDto = new
            {
                idComentario = comentario.IdComentario,
                idChamado = comentario.IdChamado,
                idUsuario = comentario.IdUsuario,
                nomeUsuario = usuario.NomeCompleto,
                mensagem = comentario.Mensagem,
                dataHoraEnvio = comentario.DataHoraEnvio,
                lido = comentario.Lido,
                ehTecnico = usuario.PerfilAcesso.NomePerfil == "Técnico" || usuario.PerfilAcesso.NomePerfil == "Administrador"
            };

            // Enviar para todos no grupo do chamado
            var groupName = $"Chamado_{idChamado}";
            await Clients.Group(groupName).SendAsync("ReceberMensagem", comentarioDto);

            _logger.LogInformation("Mensagem enviada no chamado {IdChamado} por usuário {IdUsuario}",
                idChamado, idUsuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar mensagem no chamado {IdChamado}", idChamado);
            throw;
        }
    }

    /// <summary>
    /// Notifica que um usuário está digitando
    /// </summary>
    public async Task UsuarioDigitando(int idChamado, string nomeUsuario)
    {
        var groupName = $"Chamado_{idChamado}";
        await Clients.OthersInGroup(groupName).SendAsync("UsuarioDigitando", nomeUsuario);
    }

    /// <summary>
    /// Para de enviar notificação de digitação
    /// </summary>
    public async Task UsuarioParouDeDigitar(int idChamado)
    {
        var groupName = $"Chamado_{idChamado}";
        await Clients.OthersInGroup(groupName).SendAsync("UsuarioParouDeDigitar");
    }

    /// <summary>
    /// Marca mensagens como lidas
    /// </summary>
    public async Task MarcarMensagensComoLidas(int idChamado, int idUsuario)
    {
        try
        {
            var mensagensNaoLidas = await _context.ComentariosChamados
                .Where(c => c.IdChamado == idChamado && c.IdUsuario != idUsuario && !c.Lido)
                .ToListAsync();

            foreach (var mensagem in mensagensNaoLidas)
            {
                mensagem.Lido = true;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("{Count} mensagens marcadas como lidas no chamado {IdChamado}",
                mensagensNaoLidas.Count, idChamado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao marcar mensagens como lidas");
        }
    }
}
