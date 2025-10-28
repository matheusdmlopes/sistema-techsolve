using System;
using System.Collections.Generic;

namespace PIM.Desktop.Models
{
    public class MetricasDashboard
    {
        public ResumoGeral ResumoGeral { get; set; }
        public PeriodoInfo Periodo { get; set; }
        public DistribuicaoInfo Distribuicao { get; set; }
    }

    public class ResumoGeral
    {
        public int TotalChamados { get; set; }
        public int ChamadosAbertos { get; set; }
        public int ChamadosEmAndamento { get; set; }
        public int ChamadosResolvidos { get; set; }
        public int ChamadosFechados { get; set; }
        public int TecnicosAtivos { get; set; }
        public TempoResolucaoInfo TempoMedioResolucao { get; set; }
        public double SatisfacaoMedia { get; set; }
        public int TotalFeedbacks { get; set; }
    }

    public class TempoResolucaoInfo
    {
        public double Horas { get; set; }
        public string Formatado { get; set; }
    }

    public class PeriodoInfo
    {
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public int TotalChamadosPeriodo { get; set; }
        public TendenciaInfo Tendencia { get; set; }
    }

    public class TendenciaInfo
    {
        public int Diferenca { get; set; }
        public double Percentual { get; set; }
        public string Direcao { get; set; }
    }

    public class DistribuicaoInfo
    {
        public List<PrioridadeCount> PorPrioridade { get; set; }
        public List<CategoriaCount> TopCategorias { get; set; }
    }

    public class PrioridadeCount
    {
        public string Prioridade { get; set; }
        public int Total { get; set; }
    }

    public class CategoriaCount
    {
        public string Categoria { get; set; }
        public int Total { get; set; }
    }
}
