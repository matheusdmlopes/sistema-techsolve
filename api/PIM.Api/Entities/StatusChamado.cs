namespace PIM.Api.Entities;

public class StatusChamado
{
    public int IdStatus { get; set; }
    public string NomeStatus { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    
    // Navigation Properties
    public ICollection<Chamado> Chamados { get; set; } = [];
}