namespace PIM.Api.Entities;

public class SugestaoIA
{
    public int IdSugestao { get; set; }
    public int IdChamado { get; set; }
    public string TextoSugestao { get; set; } = string.Empty;
    public DateTime DataHoraGeracao { get; set; } = DateTime.Now;
    public bool FoiAceita { get; set; } = false;
    
    // Navigation Properties
    public Chamado Chamado { get; set; } = null!;
}