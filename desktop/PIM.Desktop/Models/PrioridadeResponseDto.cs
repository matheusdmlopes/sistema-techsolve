namespace PIM.Desktop.Models
{
    /// <summary>
    /// DTO para resposta da API de Prioridades (GET /api/Prioridades)
    /// </summary>
    public class PrioridadeResponseDto
    {
        public int IdPrioridade { get; set; }
        public string Nivel { get; set; }
        public string Descricao { get; set; }
    }
}
