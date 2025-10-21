using System.ComponentModel.DataAnnotations;

namespace PIM.Api.Entities;

/// <summary>
/// Representa um chamado técnico (ticket) aberto por um usuário
/// </summary>
/// <remarks>
/// Esta entidade é central no sistema de suporte.
/// Contém todas as informações sobre um problema reportado, incluindo:
/// - Dados básicos (título, descrição, datas)
/// - Classificação (categoria, prioridade, status)
/// - Relacionamentos (usuário criador, técnico responsável)
/// - Histórico de alterações
/// - Anexos, sugestões da IA, comentários e feedback
/// </remarks>
public class Chamado
{
    /// <summary>
    /// Identificador único do chamado
    /// </summary>
    public int IdChamado { get; set; }

    /// <summary>
    /// Título resumido do problema (máximo 100 caracteres)
    /// </summary>
    [Required(ErrorMessage = "Título é obrigatório")]
    [StringLength(100, ErrorMessage = "Título deve ter no máximo 100 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada do problema (mínimo 10 caracteres)
    /// </summary>
    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(int.MaxValue, MinimumLength = 10, ErrorMessage = "Descrição deve ter pelo menos 10 caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora em que o chamado foi aberto (UTC)
    /// </summary>
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data e hora em que o chamado foi fechado/resolvido (UTC), null se ainda aberto
    /// </summary>
    public DateTime? DataFechamento { get; set; }

    /// <summary>
    /// Descrição da solução final aplicada ao problema
    /// </summary>
    public string? SolucaoFinal { get; set; }

    // Foreign Keys

    /// <summary>
    /// ID do usuário que criou o chamado
    /// </summary>
    [Required(ErrorMessage = "Usuário criador é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "IdUsuarioCriador deve ser um valor válido")]
    public int IdUsuarioCriador { get; set; }

    /// <summary>
    /// ID do técnico responsável por resolver o chamado (null se não atribuído)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "IdUsuarioTecnico deve ser um valor válido")]
    public int? IdUsuarioTecnico { get; set; }

    /// <summary>
    /// ID da categoria do problema (Hardware, Software, Rede, etc.)
    /// </summary>
    [Required(ErrorMessage = "Categoria é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "IdCategoria deve ser um valor válido")]
    public int IdCategoria { get; set; }

    /// <summary>
    /// ID da prioridade do chamado (Baixa, Média, Alta, Crítica)
    /// </summary>
    [Required(ErrorMessage = "Prioridade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "IdPrioridade deve ser um valor válido")]
    public int IdPrioridade { get; set; }

    /// <summary>
    /// ID do status atual do chamado (Aberto, Em Andamento, Resolvido, etc.)
    /// </summary>
    [Required(ErrorMessage = "Status é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "IdStatus deve ser um valor válido")]
    public int IdStatus { get; set; }

    // Navigation Properties

    /// <summary>
    /// Usuário que criou o chamado
    /// </summary>
    public Usuario UsuarioCriador { get; set; } = null!;

    /// <summary>
    /// Técnico responsável pelo atendimento (null se não atribuído)
    /// </summary>
    public Usuario? UsuarioTecnico { get; set; }

    /// <summary>
    /// Categoria do problema
    /// </summary>
    public Categoria Categoria { get; set; } = null!;

    /// <summary>
    /// Prioridade do chamado
    /// </summary>
    public Prioridade Prioridade { get; set; } = null!;

    /// <summary>
    /// Status atual do chamado
    /// </summary>
    public StatusChamado Status { get; set; } = null!;

    /// <summary>
    /// Histórico de alterações de status do chamado
    /// </summary>
    public ICollection<HistoricoChamado> Historicos { get; set; } = [];

    /// <summary>
    /// Sugestões geradas pela IA para resolução do problema
    /// </summary>
    public ICollection<SugestaoIA> Sugestoes { get; set; } = [];

    /// <summary>
    /// Arquivos anexados ao chamado
    /// </summary>
    public ICollection<Anexo> Anexos { get; set; } = [];

    /// <summary>
    /// Comentários/mensagens trocadas entre usuário e técnico
    /// </summary>
    public ICollection<ComentarioChamado> Comentarios { get; set; } = [];

    /// <summary>
    /// Feedback do usuário sobre a resolução (nota e comentário)
    /// </summary>
    public Feedback? Feedback { get; set; }
}