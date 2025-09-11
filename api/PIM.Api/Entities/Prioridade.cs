namespace PIM.Api.Entities;

public class Prioridade
{
    public int IdPrioridade { get; set; }
    public string NivelPrioridade { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    
    // Navigation Properties
    public ICollection<Chamado> Chamados { get; set; } = [];
}