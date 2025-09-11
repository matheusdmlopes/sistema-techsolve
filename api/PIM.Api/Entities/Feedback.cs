using System.ComponentModel.DataAnnotations;
using PIM.Api.Data.Validations;

namespace PIM.Api.Entities;

public class Feedback
{
    public int IdFeedback { get; set; }
    
    [Required(ErrorMessage = "Chamado é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "IdChamado deve ser um valor válido")]
    public int IdChamado { get; set; }
    
    [Required(ErrorMessage = "Nota é obrigatória")]
    [NotaValidation]
    public int Nota { get; set; }
    
    [StringLength(500, ErrorMessage = "Comentário deve ter no máximo 500 caracteres")]
    public string? Comentario { get; set; }
    
    public DateTime DataHoraFeedback { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public Chamado Chamado { get; set; } = null!;
}