namespace PIM.Api.Entities;

public class PerfilAcesso
{
    public int IdPerfil { get; set; }
    public string NomePerfil { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    
    // Navigation Properties
    public ICollection<Usuario> Usuarios { get; set; } = [];
}