using System.Collections.Generic;

namespace PIM.Desktop.Models
{
    public class RelatorioSatisfacao
    {
        public int TotalFeedbacks { get; set; }
        public double NotaMedia { get; set; }
        public Dictionary<int, int> DistribuicaoNotas { get; set; }
        public double PercentualSatisfacao { get; set; }
        public int FeedbacksSatisfatorios { get; set; }
        public int FeedbacksInsatisfatorios { get; set; }
        public PeriodoRelatorio Periodo { get; set; }
    }

    public class PeriodoRelatorio
    {
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
    }
}
