namespace PIM.Api.DTOs.Responses
{
    /// <summary>
    /// DTO simplificado para listagem de chamados
    /// </summary>
    public class ChamadoDto
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
        public UsuarioSimplificadoDto UsuarioCriador { get; set; } = new();
        public UsuarioSimplificadoDto? UsuarioTecnico { get; set; }
    }
}
