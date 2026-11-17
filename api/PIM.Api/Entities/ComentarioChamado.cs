using System.ComponentModel.DataAnnotations;

namespace PIM.Api.Entities;

/// <summary>
/// Representa um comentário/mensagem em um chamado (chat entre técnico e usuário)
/// </summary>
public class ComentarioChamado
{
    public int IdComentario { get; set; }

    [Required(ErrorMessage = "Chamado é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "IdChamado deve ser um valor válido")]
    public int IdChamado { get; set; }

    [Required(ErrorMessage = "Usuário é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "IdUsuario deve ser um valor válido")]
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "Mensagem é obrigatória")]
    [StringLength(2000, MinimumLength = 1, ErrorMessage = "Mensagem deve ter entre 1 e 2000 caracteres")]
    public string Mensagem { get; set; } = string.Empty;

    public DateTime DataHoraEnvio { get; set; } = DateTime.Now;

    public bool Lido { get; set; } = false;

    // Navigation Properties
    public Chamado Chamado { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
