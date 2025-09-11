namespace PIM.Api.Entities;

public class Categoria
{
    public int IdCategoria { get; set; }
    public string NomeCategoria { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    
    // Navigation Properties
    public ICollection<Chamado> Chamados { get; set; } = [];
}