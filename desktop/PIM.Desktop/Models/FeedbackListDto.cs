using System;

namespace PIM.Desktop.Models
{
    public class FeedbackListDto
    {
        public int Id { get; set; }
        public int IdChamado { get; set; }
        public string TituloChamado { get; set; }
        public int Nota { get; set; }
        public string Comentario { get; set; }
        public DateTime DataHoraFeedback { get; set; }
        public string Solicitante { get; set; }
        public string Tecnico { get; set; }
    }
}
