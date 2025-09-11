namespace PIM.Api.Entities;

public class HistoricoChamado
{
    public int IdHistorico { get; set; }
    public int IdChamado { get; set; }
    public int? IdTecnico { get; set; }
    public string? StatusAnterior { get; set; }
    public string StatusAtual { get; set; } = string.Empty;
    public DateTime DataHoraAlteracao { get; set; } = DateTime.UtcNow;
    public string? Observacao { get; set; }
    
    // Navigation Properties
    public Chamado Chamado { get; set; } = null!;
    public Usuario? Tecnico { get; set; }
}