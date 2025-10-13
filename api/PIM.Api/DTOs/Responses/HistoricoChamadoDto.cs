namespace PIM.Api.DTOs.Responses
{
    public class HistoricoChamadoDto
    {
        public int IdHistorico { get; set; }
        public DateTime DataMudanca { get; set; }
        public string? StatusAnterior { get; set; }
        public string StatusNovo { get; set; } = string.Empty;
        public string? Observacao { get; set; }
        public UsuarioSimplificadoDto Tecnico { get; set; } = new();
    }
}
