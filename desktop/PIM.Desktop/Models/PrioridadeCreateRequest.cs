namespace PIM.Desktop.Models
{
    /// <summary>
    /// DTO para criação de nova Prioridade
    /// </summary>
    public class PrioridadeCreateRequest
    {
        public string Nivel { get; set; }
        public string Descricao { get; set; }
    }
}
