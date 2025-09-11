namespace PIM.Api.Entities;

public class Anexo
{
    public int IdAnexo { get; set; }
    public int IdChamado { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public string CaminhoArquivo { get; set; } = string.Empty;
    public DateTime DataUpload { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public Chamado Chamado { get; set; } = null!;
}