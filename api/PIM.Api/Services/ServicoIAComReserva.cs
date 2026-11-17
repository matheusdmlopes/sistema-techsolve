namespace PIM.Api.Services;

/// <summary>
/// Usa o serviço de IA principal e, se ele falhar, responde com o serviço reserva.
/// </summary>
/// <remarks>
/// Garante que a triagem continue funcionando quando a OpenAI não responde
/// (sem internet, chave inválida ou limite de uso atingido).
/// </remarks>
public class ServicoIAComReserva : IServicoIA
{
    private readonly IServicoIA _principal;
    private readonly IServicoIA _reserva;
    private readonly ILogger<ServicoIAComReserva> _logger;

    public ServicoIAComReserva(IServicoIA principal, IServicoIA reserva, ILogger<ServicoIAComReserva> logger)
    {
        _principal = principal;
        _reserva = reserva;
        _logger = logger;
    }

    public Task<string> GerarSugestaoAsync(string titulo, string descricao, string? categoriaAtual, string? prioridadeAtual) =>
        ExecutarAsync(ia => ia.GerarSugestaoAsync(titulo, descricao, categoriaAtual, prioridadeAtual));

    public Task<string> CategorizarAsync(string titulo, string descricao, IReadOnlyList<string> categorias) =>
        ExecutarAsync(ia => ia.CategorizarAsync(titulo, descricao, categorias));

    public Task<string> PriorizarAsync(string titulo, string descricao, IReadOnlyList<string> prioridades) =>
        ExecutarAsync(ia => ia.PriorizarAsync(titulo, descricao, prioridades));

    public Task<string> AnalisarPreliminarAsync(string titulo, string descricao, IReadOnlyList<string> categorias, IReadOnlyList<string> prioridades) =>
        ExecutarAsync(ia => ia.AnalisarPreliminarAsync(titulo, descricao, categorias, prioridades));

    private async Task<string> ExecutarAsync(Func<IServicoIA, Task<string>> operacao)
    {
        try
        {
            return await operacao(_principal);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Serviço de IA principal falhou; usando a base de conhecimento local.");
            return await operacao(_reserva);
        }
    }
}
