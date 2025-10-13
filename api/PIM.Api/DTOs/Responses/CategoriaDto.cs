namespace PIM.Api.DTOs.Responses
{
    public class CategoriaDto
    {
        public int IdCategoria { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }
}
