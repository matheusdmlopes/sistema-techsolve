using System.ComponentModel.DataAnnotations;

namespace PIM.Api.Entities;

public class Chamado
{
    public int IdChamado { get; set; }
    
    [Required(ErrorMessage = "Título é obrigatório")]
    [StringLength(100, ErrorMessage = "Título deve ter no máximo 100 caracteres")]
    public string Titulo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(int.MaxValue, MinimumLength = 10, ErrorMessage = "Descrição deve ter pelo menos 10 caracteres")]
    public string Descricao { get; set; } = string.Empty;
    
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    public DateTime? DataFechamento { get; set; }
    public string? SolucaoFinal { get; set; }
    
    // Foreign Keys
    [Required(ErrorMessage = "Usuário criador é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "IdUsuarioCriador deve ser um valor válido")]
    public int IdUsuarioCriador { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "IdUsuarioTecnico deve ser um valor válido")]
    public int? IdUsuarioTecnico { get; set; }
    
    [Required(ErrorMessage = "Categoria é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "IdCategoria deve ser um valor válido")]
    public int IdCategoria { get; set; }
    
    [Required(ErrorMessage = "Prioridade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "IdPrioridade deve ser um valor válido")]
    public int IdPrioridade { get; set; }
    
    [Required(ErrorMessage = "Status é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "IdStatus deve ser um valor válido")]
    public int IdStatus { get; set; }
    
    // Navigation Properties
    public Usuario UsuarioCriador { get; set; } = null!;
    public Usuario? UsuarioTecnico { get; set; }
    public Categoria Categoria { get; set; } = null!;
    public Prioridade Prioridade { get; set; } = null!;
    public StatusChamado Status { get; set; } = null!;
    public ICollection<HistoricoChamado> Historicos { get; set; } = [];
    public ICollection<SugestaoIA> Sugestoes { get; set; } = [];
    public ICollection<Anexo> Anexos { get; set; } = [];
    public ICollection<ComentarioChamado> Comentarios { get; set; } = [];
    public Feedback? Feedback { get; set; }
}