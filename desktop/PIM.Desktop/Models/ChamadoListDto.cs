using System;

namespace PIM.Desktop.Models
{
    // DTO "achatado" para exibição no DataGridView
    public class ChamadoListDto
    {
        public int IdChamado { get; set; }
        public string Titulo { get; set; }
        public string Categoria { get; set; }
        public string Prioridade { get; set; }
        public string Status { get; set; }
        public string UsuarioCriador { get; set; }
        public string Tecnico { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
    }
}
