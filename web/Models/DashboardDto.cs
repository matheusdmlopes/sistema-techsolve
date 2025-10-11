namespace PIM.Web.Models
{
    /// <summary>
    /// DTO para métricas do dashboard
    /// </summary>
    public class DashboardMetricasDto
    {
        public int TotalChamados { get; set; }
        public int ChamadosAbertos { get; set; }
        public int ChamadosEmAndamento { get; set; }
        public int ChamadosResolvidos { get; set; }
        public int ChamadosFechados { get; set; }
        public int TecnicosAtivos { get; set; }
        public double? TempoMedioResolucaoHoras { get; set; }
        public List<ChamadoRecenteDto> ChamadosRecentes { get; set; } = new();
        public Dictionary<string, int>? ChamadosPorCategoria { get; set; }
    }

    public class ChamadoRecenteDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Prioridade { get; set; } = string.Empty;
        public DateTime DataAbertura { get; set; }
    }
}
