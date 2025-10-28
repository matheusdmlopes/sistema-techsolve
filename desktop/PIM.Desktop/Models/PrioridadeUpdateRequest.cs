namespace PIM.Desktop.Models
{
    /// <summary>
    /// DTO para atualização de Prioridade existente
    /// </summary>
    public class PrioridadeUpdateRequest
    {
        public string Nivel { get; set; }
        public string Descricao { get; set; }
    }
}
