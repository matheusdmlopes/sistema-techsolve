namespace PIM.Api.Entities;

/// <summary>
/// Representa um possível status de um chamado técnico
/// </summary>
/// <remarks>
/// Define os diferentes estados que um chamado pode ter durante seu ciclo de vida.
/// Exemplos: Aberto, Em Análise, Em Andamento, Aguardando Usuário, Resolvido, Fechado, Cancelado.
/// </remarks>
public class StatusChamado
{
    /// <summary>
    /// Identificador único do status
    /// </summary>
    public int IdStatus { get; set; }

    /// <summary>
    /// Nome do status (ex: "Aberto", "Em Andamento", "Resolvido")
    /// </summary>
    public string NomeStatus { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada do que significa este status (opcional)
    /// </summary>
    public string? Descricao { get; set; }

    // Navigation Properties

    /// <summary>
    /// Chamados que estão atualmente neste status
    /// </summary>
    public ICollection<Chamado> Chamados { get; set; } = [];
}