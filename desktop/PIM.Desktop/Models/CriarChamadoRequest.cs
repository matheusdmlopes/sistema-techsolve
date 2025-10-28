namespace PIM.Desktop.Models
{
    public class CriarChamadoRequest
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public int IdCategoria { get; set; }
        public int IdPrioridade { get; set; }
        public int? IdUsuarioCriador { get; set; }
    }
}
