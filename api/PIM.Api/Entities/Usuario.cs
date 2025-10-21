using System.ComponentModel.DataAnnotations;

namespace PIM.Api.Entities;

/// <summary>
/// Representa um usuário do sistema (pode ser Solicitante, Técnico ou Administrador)
/// </summary>
/// <remarks>
/// Esta entidade armazena informações de autenticação e perfil dos usuários.
/// Senha é armazenada com hash BCrypt para segurança.
/// Usuários podem ter diferentes perfis de acesso que determinam suas permissões.
/// </remarks>
public class Usuario
{
    /// <summary>
    /// Identificador único do usuário
    /// </summary>
    public int IdUsuario { get; set; }

    /// <summary>
    /// Nome completo do usuário (máximo 100 caracteres)
    /// </summary>
    [Required(ErrorMessage = "Nome completo é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome completo deve ter no máximo 100 caracteres")]
    public string NomeCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário, usado para login (deve ser único)
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter formato válido")]
    [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário (armazenada com hash BCrypt)
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 255 caracteres")]
    public string Senha { get; set; } = string.Empty;

    /// <summary>
    /// Telefone de contato do usuário (opcional)
    /// </summary>
    [Phone(ErrorMessage = "Telefone deve ter formato válido")]
    [StringLength(15, ErrorMessage = "Telefone deve ter no máximo 15 caracteres")]
    public string? Telefone { get; set; }

    /// <summary>
    /// Data e hora de cadastro do usuário no sistema (UTC)
    /// </summary>
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data e hora do último login do usuário (null se nunca fez login)
    /// </summary>
    public DateTime? UltimoLogin { get; set; }

    /// <summary>
    /// Indica se é o primeiro acesso do usuário (usado para forçar troca de senha)
    /// </summary>
    public bool PrimeiroAcesso { get; set; } = true;

    /// <summary>
    /// Indica se o usuário está ativo no sistema (usuários inativos não podem fazer login)
    /// </summary>
    public bool Ativo { get; set; } = true;

    // Foreign Keys

    /// <summary>
    /// ID do setor ao qual o usuário pertence (TI, RH, Financeiro, etc.)
    /// </summary>
    [Required(ErrorMessage = "Setor é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "IdSetor deve ser um valor válido")]
    public int IdSetor { get; set; }

    /// <summary>
    /// ID do perfil de acesso do usuário (1=Admin, 2=Técnico, 3=Solicitante)
    /// </summary>
    [Required(ErrorMessage = "Perfil de acesso é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "IdPerfil deve ser um valor válido")]
    public int IdPerfil { get; set; }

    // Navigation Properties

    /// <summary>
    /// Setor ao qual o usuário está vinculado
    /// </summary>
    public Setor Setor { get; set; } = null!;

    /// <summary>
    /// Perfil de acesso que define as permissões do usuário
    /// </summary>
    public PerfilAcesso PerfilAcesso { get; set; } = null!;

    /// <summary>
    /// Chamados criados por este usuário (quando atua como solicitante)
    /// </summary>
    public ICollection<Chamado> ChamadosCriados { get; set; } = [];

    /// <summary>
    /// Chamados atribuídos a este usuário (quando atua como técnico)
    /// </summary>
    public ICollection<Chamado> ChamadosAtendidos { get; set; } = [];

    /// <summary>
    /// Histórico de alterações de status realizadas por este usuário
    /// </summary>
    public ICollection<HistoricoChamado> HistoricosChamado { get; set; } = [];
}