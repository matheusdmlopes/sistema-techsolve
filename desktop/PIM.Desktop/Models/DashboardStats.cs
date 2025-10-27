namespace PIM.Desktop.Models
{
    /// <summary>
    /// Estatísticas do dashboard
    /// </summary>
    public class DashboardStats
    {
        public int TotalChamados { get; set; }
        public int ChamadosAbertos { get; set; }
        public int ChamadosEmAndamento { get; set; }
        public int ChamadosResolvidos { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalTecnicos { get; set; }
        public double MediaSatisfacao { get; set; }
    }
}
