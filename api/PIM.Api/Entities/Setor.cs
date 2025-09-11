namespace PIM.Api.Entities;

public class Setor
{
    public int IdSetor { get; set; }
    public string NomeSetor { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    
    // Navigation Properties
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}