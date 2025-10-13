namespace PIM.Api.DTOs.Responses
{
    /// <summary>
    /// DTO completo com todos os relacionamentos do chamado
    /// </summary>
    public class ChamadoDetalhadoDto
    {
        public int IdChamado { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public string? SolucaoFinal { get; set; }

        public CategoriaDto Categoria { get; set; } = new();
        public PrioridadeDto Prioridade { get; set; } = new();
        public StatusDto Status { get; set; } = new();
        public UsuarioComSetorDto UsuarioCriador { get; set; } = new();
        public UsuarioSimplificadoDto? UsuarioTecnico { get; set; }

        public List<HistoricoChamadoDto> Historicos { get; set; } = new();
        public List<AnexoDto> Anexos { get; set; } = new();
        public List<SugestaoIADto> Sugestoes { get; set; } = new();
        public FeedbackDto? Feedback { get; set; }
    }
}
