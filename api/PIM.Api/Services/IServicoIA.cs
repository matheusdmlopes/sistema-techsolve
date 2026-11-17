namespace PIM.Api.Services;

/// <summary>
/// Contrato do serviço de Inteligência Artificial usado na triagem de chamados.
/// </summary>
/// <remarks>
/// Existem duas implementações:
/// - <see cref="ServicoIASimulada"/>: funciona sem internet, a partir de uma base de conhecimento local (padrão).
/// - <see cref="ServicoIAOpenAI"/>: usa o modelo GPT-3.5-turbo da OpenAI (exige internet e chave de API).
/// A escolha é feita pela configuração "IA:Modo" (Simulada ou OpenAI).
/// </remarks>
public interface IServicoIA
{
    /// <summary>
    /// Gera uma sugestão de solução em texto livre para o técnico que está atendendo o chamado.
    /// </summary>
    Task<string> GerarSugestaoAsync(string titulo, string descricao, string? categoriaAtual, string? prioridadeAtual);

    /// <summary>
    /// Devolve o nome de uma das categorias informadas, a que melhor descreve o problema.
    /// </summary>
    Task<string> CategorizarAsync(string titulo, string descricao, IReadOnlyList<string> categorias);

    /// <summary>
    /// Devolve o nome de uma das prioridades informadas, a mais adequada ao impacto do problema.
    /// </summary>
    Task<string> PriorizarAsync(string titulo, string descricao, IReadOnlyList<string> prioridades);

    /// <summary>
    /// Analisa o problema antes da abertura do chamado.
    /// </summary>
    /// <returns>
    /// Texto no formato esperado pelo IAPreliminarController:
    /// linhas "CATEGORIA:", "PRIORIDADE:", "PODE_RESOLVER:" (SIM ou NÃO) e "SOLUÇÃO:" seguida das orientações.
    /// </returns>
    Task<string> AnalisarPreliminarAsync(string titulo, string descricao, IReadOnlyList<string> categorias, IReadOnlyList<string> prioridades);
}
