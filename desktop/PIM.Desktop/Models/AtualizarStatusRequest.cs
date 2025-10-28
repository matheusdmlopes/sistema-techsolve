namespace PIM.Desktop.Models
{
    public class AtualizarStatusRequest
    {
        public int IdStatus { get; set; }
        public string Observacao { get; set; }
        public string SolucaoFinal { get; set; }
        public int? IdUsuarioTecnico { get; set; }
    }
}
