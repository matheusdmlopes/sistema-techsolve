namespace PIM.Api.DTOs.Responses
{
    public class PrioridadeDto
    {
        public int IdPrioridade { get; set; }
        public string Nivel { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }
}
