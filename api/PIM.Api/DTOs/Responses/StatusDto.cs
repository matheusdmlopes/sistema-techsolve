namespace PIM.Api.DTOs.Responses
{
    public class StatusDto
    {
        public int IdStatus { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }
}
